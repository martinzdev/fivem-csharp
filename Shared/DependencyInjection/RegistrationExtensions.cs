using System;
using System.Linq;

namespace core.Shared.DependencyInjection
{
  public static class RegistrationExtensions
  {
    public static Registration SingleInstance(this Registration registration)
    {
      registration.SingleInstance = true;

      return registration;
    }

    public static Registration As<T>(this Registration registration)
    {
      var type = typeof(T);
      if (!registration.Types.Contains(type))
        registration.Types.Add(type);

      return registration;
    }

    public static Registration As(this Registration registration, params Type[] types)
    {
      foreach (var type in types)
      {
        if (!registration.Types.Contains(type))
          registration.Types.Add(type);
      }

      return registration;
    }

    public static Registration AsImplementedInterfaces(this Registration registration)
    {
      var interfaces = registration.InstanceType.GetInterfaces();
      
      foreach (var interfaceType in interfaces)
      {
        if (!registration.Types.Contains(interfaceType))
          registration.Types.Add(interfaceType);
      }

      return registration;
    }
  }
}