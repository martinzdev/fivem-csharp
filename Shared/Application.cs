using System;
using System.Collections.Generic;
using System.Reflection;

namespace core.Shared
{
  [AttributeUsage(AttributeTargets.Class)]
  public class ServiceAttribute : Attribute
  {
    public Type InterfaceType { get; set; }
    
    public ServiceAttribute() { }
    
    public ServiceAttribute(Type interfaceType)
    {
      InterfaceType = interfaceType;
    }
  }

  [AttributeUsage(AttributeTargets.Class)]
  public class SingletonAttribute : Attribute
  {
  }

  public static class Application
  {
    private static Dictionary<Type, Func<object>> _registrations;
    private static Dictionary<Type, object> _singletons;
    private static bool _built;

    public static void StartRegistration()
    {
      _registrations = new Dictionary<Type, Func<object>>();
      _singletons = new Dictionary<Type, object>();
      _built = false;
    }

    public static void RegisterService<TInterface, TImplementation>()
      where TInterface : class
      where TImplementation : class, TInterface, new()
    {
      if (_built)
        throw new Exception("Service is already registered");

      var typeInterface = typeof(TInterface);
      if (_registrations.ContainsKey(typeInterface))
        throw new Exception($"Service {typeInterface.Name} already registered.");

      _registrations[typeInterface] = () => new TImplementation();
    }

    public static void AutoRegisterServices()
    {
      if (_built)
        throw new Exception("Application is already built");

      var assembly = Assembly.GetExecutingAssembly();
      var types = assembly.GetTypes();

      foreach (var type in types)
      {
        var serviceAttribute = type.GetCustomAttribute<ServiceAttribute>();
        if (serviceAttribute != null)
        {
          var interfaceType = serviceAttribute.InterfaceType ?? type;
          var isSingleton = type.GetCustomAttribute<SingletonAttribute>() != null;

          if (_registrations.ContainsKey(interfaceType))
            continue; // Skip if already registered

          if (isSingleton)
          {
            // Register as singleton
            var instance = Activator.CreateInstance(type);
            _singletons[interfaceType] = instance;
            _registrations[interfaceType] = () => instance;
          }
          else
          {
            // Register as transient
            _registrations[interfaceType] = () => Activator.CreateInstance(type);
          }
        }
      }
    }

    public static void Build()
    {
      if (_built) return;
      // Optionally, instantiate all singletons here if you want eager loading
      _built = true;
    }

    public static T Get<T>() where T : class
    {
      var type = typeof(T);
      if (!_built)
        throw new Exception("Application not built. Call Build() first.");

      if (!_singletons.ContainsKey(type))
      {
        if (!_registrations.ContainsKey(type))
          throw new Exception($"Service {type.Name} not registered.");

        var instance = _registrations[type]();
        _singletons[type] = instance;
      }
      return (T)_singletons[type];
    }

    public static void Reset()
    {
      _registrations?.Clear();
      _singletons?.Clear();
      _built = false;
    }
  }
}