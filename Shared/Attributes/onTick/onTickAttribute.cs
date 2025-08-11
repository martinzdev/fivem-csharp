using System;

namespace core.Shared.Attributes.onTick
{
  [AttributeUsage(AttributeTargets.Method)]
  public class onTickAttribute : Attribute
  {
    public int Interval { get; }
    public string Name { get; }

    public onTickAttribute(int interval = 0, string name = "")
    {
      Interval = interval;
      Name = name;
    }
  }
}