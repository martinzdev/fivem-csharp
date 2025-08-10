using System;

namespace core.Shared.DependencyInjection
{
  [AttributeUsage(AttributeTargets.Class)]
  public class ServiceAttribute : Attribute
  {
    public Lifecycle Lifecycle { get; }
    public Type[] Interfaces { get; }
  
    public ServiceAttribute(Lifecycle lifecycle = Lifecycle.Singleton, params Type[] interfaces)
    {
      Lifecycle = lifecycle;
      Interfaces = interfaces;
    }
  }

  [AttributeUsage(AttributeTargets.Class)]
  public class ControllerAttribute : Attribute
  {
    public Lifecycle Lifecycle { get; }
  
    public ControllerAttribute(Lifecycle lifecycle = Lifecycle.Singleton)
    {
      Lifecycle = lifecycle;
    }
  }

  public enum Lifecycle
  {
    Singleton,
    Transient
  }
}