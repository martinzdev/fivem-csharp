using System;

namespace core.Client.Services
{
    public class Logger : ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine($"[INFO] {message}");
        }

        public void Error(string message)
        {
            Console.WriteLine($"[ERROR] {message}");
        }

        public void Warning(string message)
        {
            Console.WriteLine($"[WARNING] {message}");
        }

        public void Debug(string message)
        {
            Console.WriteLine($"[DEBUG] {message}");
        }
    }
}
