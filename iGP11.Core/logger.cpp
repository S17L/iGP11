#include "stdafx.h"
#include "logger.h"

std::string getLogTypeName(core::logging::LogLevel logLevel) {
	const char* logLevels[] = { "INFO", "DEBUG", "WARN", "ERROR" };
	return std::string(logLevels[logLevel]);
}

core::logging::FileLoggingTarget::FileLoggingTarget(std::string filePath) {
	auto directory = core::toWString(core::file::getDirectory(filePath));
	::CreateDirectory(directory.c_str(), NULL);
	_file.open(filePath, std::ios::out | std::ios::app | std::ios::binary);
}

core::logging::FileLoggingTarget::~FileLoggingTarget() {
	_file.close();
}

void core::logging::FileLoggingTarget::handle(const std::string &text) {
	std::lock_guard<std::mutex> lock(_mutex);
	_file << text << std::endl;
}

core::logging::ConcreteLogger::ConcreteLogger(std::string source, core::time::ITimeProvider *timeProvider)
	: _source(source), _timeProvider(timeProvider) {}

void core::logging::ConcreteLogger::detach(const ILogTextAppender *appender) {
	std::lock_guard<std::mutex> lock(_mutex);
	for (auto iterator = _appenders.begin(); iterator != _appenders.end();) {
		if ((*iterator).get() == appender) {
			iterator = _appenders.erase(iterator);
			break;
		} else {
			++iterator;
		}
	}
}

void core::logging::ConcreteLogger::addTarget(core::logging::ILoggingTarget *target) {
	_targets.push_back(target);
}

std::unique_ptr<core::disposing::IDisposable> core::logging::ConcreteLogger::runInScope(std::unique_ptr<core::logging::ILogTextAppender> appender) {
	std::lock_guard<std::mutex> lock(_mutex);
    std::shared_ptr<core::logging::ILogTextAppender> logTextAppender = std::move(appender);
	_appenders.push_back(logTextAppender);

	return std::unique_ptr<core::disposing::IDisposable>(new ConcreteLoggerAppenderScope(this, logTextAppender));
}

void core::logging::ConcreteLogger::log(const std::string &text) {
	log(core::logging::information, text);
}

void core::logging::ConcreteLogger::log(core::logging::LogLevel logType, const std::string &text) {
#if NDEBUG
	if (logType == debug) {
		return;
	}
#endif

	std::lock_guard<std::mutex> lock(_mutex);
	std::stringstream stream;
	std::string logText;
	for (auto appender : _appenders) {
		appender->append(logText);
	}

	logText.append(text);
	stream << core::time::toString(_timeProvider->getTime()) << '\t'
		<< '[' << std::setw(5) << std::setfill('0') << std::this_thread::get_id() << ']' << '\t'
		<< ::getLogTypeName(logType) << '\t'
		<< _source << '\t'
		<< logText;

	logText = stream.str();
	for (auto target : _targets) {
		target->handle(logText);
	}
}

std::unique_ptr<core::logging::ILogger> core::logging::LoggerFactory::create(std::string source) {
	auto logger = new core::logging::ConcreteLogger(source, _timeProvider);
	logger->addTarget(_loggingTarget);

	return std::unique_ptr<core::logging::ILogger>(logger);
}