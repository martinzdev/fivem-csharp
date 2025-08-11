using core.Shared.DependencyInjection;
using core.Shared.Logger;

namespace core.Server.Utils.Logger
{
  [Service(Lifecycle.Singleton, typeof(ILogger))]
  public class Logger : ILogger
  {
    public void Info(string message)
    {
      CitizenFX.Core.Debug.WriteLine($"[INFO] {message}");
    }

    public void Error(string message)
    {
      CitizenFX.Core.Debug.WriteLine($"[ERROR] {message}");
    }

    public void Warning(string message)
    {
      CitizenFX.Core.Debug.WriteLine($"[WARNING] {message}");
    }

    public void Debug(string message)
    {
      CitizenFX.Core.Debug.WriteLine($"[DEBUG] {message}");
    }
  }
}