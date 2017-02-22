using System;
using System.Xml;

using iGP11.Library;

using log4net;
using log4net.Config;

namespace iGP11.Tool.Bootstrapper.Log4net
{
    public class Log4NetAdapter : Logger
    {
        static Log4NetAdapter()
        {
            GlobalContext.Properties["logFilePath"] = $@"logs\log_{DateTime.Now.Ticks}";
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(Configuration.Log4net);
            XmlConfigurator.Configure(xmlDocument.DocumentElement);
        }

        public override void Log(LogLevel logLevel, string prefix, string message)
        {
            var logger = LogManager.GetLogger("iGP11");
            message = prefix.IsNullOrEmpty()
                          ? message
                          : $"{prefix}: {message}";

            switch (logLevel)
            {
                case LogLevel.Debug:
                    logger.Debug(message);
                    break;
                case LogLevel.Error:
                    logger.Error(message);
                    break;
                case LogLevel.Information:
                    logger.Info(message);
                    break;
                case LogLevel.Warn:
                    logger.Warn(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }
    }
}