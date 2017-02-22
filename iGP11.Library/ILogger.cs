namespace iGP11.Library
{
    public interface ILogger
    {
        void Log(string message);

        void Log(LogLevel logLevel, string message);

        void Log(LogLevel logLevel, string prefix, string message);
    }
}