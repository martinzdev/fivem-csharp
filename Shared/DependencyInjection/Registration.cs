using System;
using System.Collections.Generic;
using System.Linq;

namespace core.Shared.DependencyInjection
{
  public class Registration<T> : Registration
  {
    public Registration()
    {
      InstanceType = typeof(T);
      Types.Add(InstanceType);
    }
  }

  public class ConcreteRegistration : Registration
  {
    public ConcreteRegistration(Type type)
    {
      InstanceType = type;
      Types.Add(type);
    }
  }

  public abstract class Registration
  {
    internal Type InstanceType { get; set; }

    internal List<Type> Types { get; } = new List<Type>();

    internal bool SingleInstance { get; set; }

    internal Func<object> Factory { get; set; }
  }
}