using System;
using core.Shared.DependencyInjection;

namespace core.Client.Utils.Logger
{
    [Service(Lifecycle.Singleton)]
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
