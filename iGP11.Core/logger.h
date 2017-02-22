#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace core {
	namespace logging {
		class ILoggingTarget {
		public:
			virtual ~ILoggingTarget() {}
			virtual void handle(const std::string &text) = 0;
		};

		class FileLoggingTarget : public ILoggingTarget {
		private:
			std::mutex _mutex;
			std::ofstream _file;
		public:
			FileLoggingTarget(std::string filePath);
			virtual ~FileLoggingTarget();
			virtual void handle(const std::string &text) override;
		};

		class ConcreteLogger : public ILogger {
			friend class ConcreteLoggerAppenderScope;
		private:
			std::mutex _mutex;
			std::list<std::shared_ptr<ILogTextAppender>> _appenders;
			std::string _source;
			core::time::ITimeProvider *_timeProvider;
			std::list<ILoggingTarget*> _targets;
			void detach(const ILogTextAppender *appender);
		public:
            ConcreteLogger(std::string source, core::time::ITimeProvider *timeProvider);
			virtual ~ConcreteLogger() {}
			void addTarget(ILoggingTarget *target);
			virtual std::unique_ptr<disposing::IDisposable> runInScope(std::unique_ptr<ILogTextAppender> appender) override;
			virtual void log(const std::string &text) override;
			virtual void log(LogLevel logLevel, const std::string &text) override;
		};

		class LoggerFactory : public ILoggerFactory {
		private:
			core::time::ITimeProvider *_timeProvider;
			ILoggingTarget *_loggingTarget;
		public:
			LoggerFactory(core::time::ITimeProvider *timeProvider, ILoggingTarget *loggingTarget)
				: _timeProvider(timeProvider), _loggingTarget(loggingTarget) {}
			virtual ~LoggerFactory() {}
			virtual std::unique_ptr<ILogger> create(std::string source) override;
		};

		class ConcreteLoggerAppenderScope final : public core::disposing::IDisposable {
		private:
            ConcreteLogger *_logger;
			std::shared_ptr<ILogTextAppender> _appender;
		public:
            ConcreteLoggerAppenderScope(ConcreteLogger *logger, std::shared_ptr<ILogTextAppender> appender)
				: _logger(logger), _appender(appender) {}
			virtual ~ConcreteLoggerAppenderScope() {
				dispose();
			}
			virtual void dispose() override {
				_logger->detach(_appender.get());
			}
		};
	}
}