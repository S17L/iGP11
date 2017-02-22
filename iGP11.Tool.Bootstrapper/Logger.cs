using iGP11.Library;

namespace iGP11.Tool.Bootstrapper
{
    public class Logger : ILogger
    {
        private static Logger _current;

        public static Logger Current
        {
            get { return _current ?? (_current = new Logger()); }
            set { _current = value; }
        }

        public void Log(string message)
        {
            Log(LogLevel.Information, message);
        }

        public void Log(LogLevel logLevel, string message)
        {
            Log(logLevel, string.Empty, message);
        }

        public virtual void Log(LogLevel logLevel, string prefix, string message)
        {
        }
    }
}