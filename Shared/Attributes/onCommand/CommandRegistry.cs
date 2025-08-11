using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using core.Shared.Logger;

namespace core.Shared.Attributes.onCommand
{
  public class onCommandRegistry
  {

    private ILogger _logger;
    public onCommandRegistry(ILogger logger)
    {
      _logger = logger;
    }
    
    public void RegisterAllCommands(IEnumerable<object> controllers)
    {
      foreach (var controller in controllers)
      {
        RegisterControllerCommands(controller);
      }
    }

    public void RegisterControllerCommands(object controller)
    {
      var methods = controller.GetType().GetMethods()
        .Where(m => m.GetCustomAttributes<onCommandAttribute>().Any());

      foreach (var method in methods)
      {
        var attribute = method.GetCustomAttribute<onCommandAttribute>();
        var handler = CreateCommandHandler(controller, method);
        
        API.RegisterCommand(attribute.CommandName, handler, attribute.Restricted);
        foreach (var alias in attribute.Aliases)
        {
          API.RegisterCommand(alias, handler, attribute.Restricted);
        }
        
        Debug.WriteLine($"Command {attribute.CommandName} registered. (Restricted: {attribute.Restricted})");
      }
    }

    private static Action<int, List<object>, string> CreateCommandHandler(object instance, MethodInfo method)
    {
      return (source, args, raw) =>
      {
        try
        {
          var parameters = method.GetParameters();
          if (
            parameters.Length == 3 &&
            parameters[0].ParameterType == typeof(int) &&
            parameters[1].ParameterType == typeof(List<object>) &&
            parameters[2].ParameterType == typeof(string)
          )
          {
            method.Invoke(instance, new object[] { source, args, raw });
            return;
          }
          
          if (
            parameters.Length == 2 &&
            parameters[0].ParameterType == typeof(int) &&
            parameters[1].ParameterType == typeof(List<object>)
          )
          {
            method.Invoke(instance, new object[] { source, args });
            return;
          }
          
          if (
            parameters.Length == 1 &&
            parameters[0].ParameterType == typeof(int)
          )
          {
            method.Invoke(instance, new object[] { source });
            return;
          }
          
          if (parameters.Length == 0)
          {
            method.Invoke(instance, null);
            return;
          }
          else
          {
             Debug.WriteLine($"Error: Invalid method signature for command handler: {method.Name}");
          }
        }
        catch (Exception e)
        {
          Debug.WriteLine($"Error creating command handler: {e}");
        }
      };
    }
  }
}