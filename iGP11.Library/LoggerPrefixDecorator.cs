namespace iGP11.Library
{
    public sealed class LoggerPrefixDecorator : ILogger
    {
        private readonly ILogger _logger;
        private readonly string _prefix;

        public LoggerPrefixDecorator(ILogger logger, string prefix)
        {
            _logger = logger;
            _prefix = prefix;
        }

        public void Log(string message)
        {
            _logger.Log(LogLevel.Information, _prefix, message);
        }

        public void Log(LogLevel logLevel, string message)
        {
            _logger.Log(logLevel, _prefix, message);
        }

        public void Log(LogLevel logLevel, string prefix, string message)
        {
            _logger.Log(logLevel, GetPrefix(prefix, _prefix), message);
        }

        private static string GetPrefix(string oldPrefix, string newPrefix)
        {
            return oldPrefix.IsNullOrEmpty()
                       ? newPrefix
                       : $"{oldPrefix} > {newPrefix}";
        }
    }
}