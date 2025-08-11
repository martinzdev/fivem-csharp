using System;
using System.Linq;
using System.Reflection;
using CitizenFX.Core;

namespace core.Shared.DependencyInjection
{
  public static class ContainerBuilderExtensions
  {
    public static void RegisterModule(this ContainerBuilder builder, Assembly assembly)
    {
      var types = assembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract)
        .ToList();

      foreach (var type in types.Where(t => t.GetCustomAttribute<ServiceAttribute>() != null))
      {
        var attr = type.GetCustomAttribute<ServiceAttribute>();
        var registration = builder.RegisterType(type);
            
        if (attr.Lifecycle == Lifecycle.Singleton)
          registration.SingleInstance();
            
        if (attr.Interfaces.Length > 0)
          registration.As(attr.Interfaces);
        else
          registration.AsImplementedInterfaces();
      }

      foreach (var type in types.Where(t => t.GetCustomAttribute<ControllerAttribute>() != null))
      {
        var attr = type.GetCustomAttribute<ControllerAttribute>();
        
        var registration = builder.RegisterType(type);
            
        if (attr.Lifecycle == Lifecycle.Singleton)
          registration.SingleInstance();
      }
    }

    public static void ResolveControllers(this Container container)
    {
      var controllerTypes = container.GetRegisteredTypes()
        .Where(t => t.GetCustomAttribute<ControllerAttribute>() != null);
        
      foreach (var type in controllerTypes)
      {
        container.Resolve(type);
      }
    }
  }
}