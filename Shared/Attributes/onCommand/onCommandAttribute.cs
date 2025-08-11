using System;

namespace core.Shared.Attributes.onCommand
{
  [AttributeUsage(AttributeTargets.Method)]
  public class onCommandAttribute : Attribute

  {
    public string CommandName { get; }
    public bool Restricted { get; }
    public string[] Aliases { get; }

    public onCommandAttribute(string commandName, bool restricted, params string[] aliases)
    {
      CommandName = commandName;
      Restricted = restricted;
      Aliases = aliases;
    }
  }
}