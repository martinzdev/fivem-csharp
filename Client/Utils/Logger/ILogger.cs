namespace core.Client.Utils.Logger
{
    public interface ILogger
    {
        void Info(string message);
        void Error(string message);
        void Warning(string message);
        void Debug(string message);
    }
}
