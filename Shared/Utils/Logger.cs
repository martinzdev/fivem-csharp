using System;
using CitizenFX.Core;

namespace core.Shared.Utils
{
  public class Logger
  {
    private string FormatMessage(string prefix, string colorCode, string message)
    {
      return $"^3[{DateTime.Now:HH:mm:ss}] {colorCode}{prefix}^7 {message}";
    }

    public void Info(string message)
    {
      Debug.WriteLine(FormatMessage("[INFO]", "^2", message)); // ^2 = green
    }

    public void Warn(string message)
    {
      Debug.WriteLine(FormatMessage("[WARN]", "^3", message)); // ^3 = yellow
    }

    public void Error(string message)
    {
      Debug.WriteLine(FormatMessage("[ERROR]", "^1", message)); // ^1 = red
    }
  }
}