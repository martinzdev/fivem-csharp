using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using core.Shared.Logger;

namespace core.Shared.Attributes.onTick
{
  public class onTickRegistry
  {
    private readonly ILogger _logger;
    private readonly List<TickHandler> _tickHandlers = new List<TickHandler>();
    private Action<Func<Task>> _registerTickHandler;

    public onTickRegistry(ILogger logger)
    {
      _logger = logger;
      _logger.Info("onTickRegistry initialized with FiveM BaseScript.Tick system");
    }

    public void SetTickRegistrationHandler(Action<Func<Task>> registerTickHandler)
    {
      _registerTickHandler = registerTickHandler;
      
      // Register the master tick handler with FiveM
      _registerTickHandler(OnMasterTick);
      _logger.Info("Master tick handler registered with FiveM BaseScript");
    }
    
    public void RegisterAllTicks(IEnumerable<object> handlerObjects)
    {
      foreach (var handlerObject in handlerObjects)
      {
        RegisterHandlerObjectTicks(handlerObject);
      }
    }

    public void RegisterHandlerObjectTicks(object handlerObject)
    {
      var methods = handlerObject.GetType().GetMethods()
        .Where(m => m.GetCustomAttributes(typeof(onTickAttribute), false).Length > 0);

      foreach (var method in methods)
      {
        var attribute = (onTickAttribute)method.GetCustomAttributes(typeof(onTickAttribute), false)[0];
        RegisterTickHandler(handlerObject, method, attribute);
      }
    }

    private void RegisterTickHandler(object instance, MethodInfo method, onTickAttribute attribute)
    {
      try
      {
        var parameters = method.GetParameters();
        
        // Validate method signature
        if (parameters.Length > 0)
        {
          _logger.Error($"Invalid method signature for onTick handler: {method.Name}. onTick methods must have no parameters.");
          return;
        }

        // Check if method returns Task (async) or void (sync)
        var isAsync = method.ReturnType == typeof(Task);
        var isSync = method.ReturnType == typeof(void);

        if (!isAsync && !isSync)
        {
          _logger.Error($"Invalid return type for onTick handler: {method.Name}. Must return void or Task.");
          return;
        }

        var tickHandler = new TickHandler
        {
          Instance = instance,
          Method = method,
          Attribute = attribute,
          IsAsync = isAsync,
          LastExecution = DateTime.UtcNow,
          IsRunning = false
        };

        _tickHandlers.Add(tickHandler);

        var handlerName = !string.IsNullOrEmpty(attribute.Name) ? attribute.Name : method.Name;
        var intervalText = attribute.Interval > 0 ? $" (every {attribute.Interval}ms)" : " (every frame)";
        
        _logger.Info($"Tick handler '{handlerName}' registered{intervalText}");
      }
      catch (Exception ex)
      {
        _logger.Error($"Error registering onTick handler {method.Name}: {ex.Message}");
      }
    }

    private async Task OnMasterTick()
    {
      try
      {
        var now = DateTime.UtcNow;
        var tasks = new List<Task>();

        foreach (var handler in _tickHandlers)
        {
          if (handler.IsRunning) continue; // Skip if already running

          bool shouldExecute = false;

          if (handler.Attribute.Interval == 0)
          {
            // Every frame execution
            shouldExecute = true;
          }
          else
          {
            // Interval-based execution
            var timeSinceLastExecution = (now - handler.LastExecution).TotalMilliseconds;
            shouldExecute = timeSinceLastExecution >= handler.Attribute.Interval;
          }

          if (shouldExecute)
          {
            handler.IsRunning = true;
            handler.LastExecution = now;

            // Execute handler
            var handlerTask = ExecuteHandlerAsync(handler);
            tasks.Add(handlerTask);
          }
        }

        // Wait for all handlers to complete (but don't block the main tick)
        if (tasks.Count > 0)
        {
          await Task.WhenAll(tasks);
        }
      }
      catch (Exception ex)
      {
        _logger.Error($"Error in OnMasterTick: {ex.Message}");
      }
    }

    private async Task ExecuteHandlerAsync(TickHandler handler)
    {
      try
      {
        if (handler.IsAsync)
        {
          await (Task)handler.Method.Invoke(handler.Instance, null);
        }
        else
        {
          handler.Method.Invoke(handler.Instance, null);
        }
      }
      catch (Exception ex)
      {
        var handlerName = !string.IsNullOrEmpty(handler.Attribute.Name) 
          ? handler.Attribute.Name 
          : handler.Method.Name;
        _logger.Error($"Error in onTick handler '{handlerName}': {ex.Message}");
      }
      finally
      {
        handler.IsRunning = false;
      }
    }

    public IEnumerable<TickHandler> GetRegisteredTicks()
    {
      return _tickHandlers.ToArray();
    }
  }

  public class TickHandler
  {
    public object Instance { get; set; }
    public MethodInfo Method { get; set; }
    public onTickAttribute Attribute { get; set; }
    public bool IsAsync { get; set; }
    public DateTime LastExecution { get; set; }
    public bool IsRunning { get; set; }
  }
}