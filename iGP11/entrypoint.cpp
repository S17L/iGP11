#include "stdafx.h"
#include "entrypoint.h"

#include "commandhandlingpolicy.h"
#include "counters.h"
#include "direct3d11pluginloader.h"
#include "gamesettingsrepository.h"
#include "hookservice.h"
#include "listener.h"
#include "logger.h"
#include "jsonserializer.h"
#include "requesthandler.h"
#include "settingsservice.h"
#include "texturecache.h"
#include "timeprovider.h"

using namespace core::logging;

core::PluginType _pluginType;
std::list<std::shared_ptr<core::IPluginLoader>> _pluginLoaders;
std::shared_ptr<core::IDirect3D11PluginLoader> _direct3D11PluginLoader;
std::unique_ptr<core::IGameSettingsRepository> _repository;
std::unique_ptr<core::ISerializer> _serializer;
std::unique_ptr<core::communication::IListener> _listener;
std::unique_ptr<core::communication::RequestHandler> _requestHandler;
std::unique_ptr<core::logging::ILoggerFactory> _loggerFactory;
std::unique_ptr<core::logging::ILoggingTarget> _fileLoggingTarget;
std::unique_ptr<core::time::ITimeProvider> _timeProvider;

std::shared_ptr<core::IPluginLoader> getPluginLoader() {
    auto pluginLoader = core::linq::makeEnumerable(_pluginLoaders)
        .where([&](const std::shared_ptr<core::IPluginLoader> &loader) { return loader->getType() == _pluginType; })
        .firstOrDefault(std::shared_ptr<core::IPluginLoader>());

    if (pluginLoader == nullptr) {
        throw new core::exception::OperationException(
            ENCRYPT_STRING("core::IPluginLoader"),
            core::stringFormat(
                ENCRYPT_STRING("plugin loader not found for %d"),
                static_cast<int>(_pluginType)));
    }

    return pluginLoader;
}

bool getGameSettings(core::ISerializer *serializer, core::dto::GameSettings &settings) {
    try {
        settings = core::SettingsService(serializer).getSettings();
        return true;
    }
    catch (core::exception::ResourceNotFoundException const &exception) {
        log(error, core::stringFormat(ENCRYPT_STRING("file not found exception occured: %s"), exception.what()));
        return false;
    }
}

bool loadPlugin(core::IPluginLoader *pluginLoader) {
    __try {
        return pluginLoader->load();
    }
    __except (EXCEPTION_EXECUTE_HANDLER) {
        return false;
    }
}

bool startPlugin(core::IPluginLoader *pluginLoader) {
    __try {
        return pluginLoader->start();
    }
    __except (EXCEPTION_EXECUTE_HANDLER) {
        return false;
    }
}

bool stopPlugin(core::IPluginLoader *pluginLoader) {
    __try {
        return pluginLoader->stop();
    }
    __except (EXCEPTION_EXECUTE_HANDLER) {
        return false;
    }
}

void startListener() {
    log(ENCRYPT_STRING("attempting to create/start listener..."));
    auto settings = _repository->load();
    _requestHandler.reset(new core::communication::RequestHandler(_serializer.get()));
    _requestHandler->addPolicy(std::shared_ptr<core::communication::ICommandHandlingPolicy>(new core::communication::GetActivationStatusCommandHandlingPolicy(getPluginLoader())));
    _requestHandler->addPolicy(std::shared_ptr<core::communication::ICommandHandlingPolicy>(new core::communication::GetProxySettingsCommandHandlingPolicy(getPluginLoader(), _repository.get(), _serializer.get())));
    _requestHandler->addPolicy(std::shared_ptr<core::communication::ICommandHandlingPolicy>(new core::communication::UpdateProxySettingsCommandHandlingPolicy(_direct3D11PluginLoader, _repository.get(), _serializer.get())));
    _listener.reset(new core::communication::TcpListener(settings.communicationAddress, settings.communicationPort, _requestHandler.get()));

    const int maxRetryCount = 16;
    const int retryDelay = 1000;
    int retryCount = 0;

    while (retryCount < maxRetryCount) {
        if (_listener->start()) {
            log(ENCRYPT_STRING("listener has been started"));
            break;
        }
        else {
            retryCount++;
            log(error, ::core::stringFormat(ENCRYPT_STRING("listener has not been started, retry: %d/%d"), retryCount, maxRetryCount));
            ::Sleep(retryDelay);
        }
    }
}

void stopListener() {
    if (_listener->stop()) {
        log(ENCRYPT_STRING("listener has been stopped"));
    }
    else {
        log(error, ENCRYPT_STRING("listener has not been stopped"));
    }
}

void initializeServer() {
    __try {
        ::startListener();
    }
    __except (EXCEPTION_EXECUTE_HANDLER) {}
}

void deinitializeServer() {
    __try {
        ::stopListener();
    }
    __except (EXCEPTION_EXECUTE_HANDLER) {}
}

DWORD __stdcall initialize(LPVOID) {
    _serializer.reset(new core::JsonSerializer());
    core::dto::GameSettings settings;
    if (!::getGameSettings(_serializer.get(), settings)) {
        return NULL;
    }

    _repository.reset(new core::GameSettingsRepository(settings));
    _timeProvider.reset(new core::time::CurrentTimeProvider());
    _fileLoggingTarget.reset(new core::logging::FileLoggingTarget(
        core::file::combine(
            settings.pluginSettings.logsDirectoryPath,
            core::stringFormat(ENCRYPT_STRING("log_%lld.txt"), _timeProvider->getTime().totalMiliseconds))));

    _loggerFactory.reset(new core::logging::LoggerFactory(_timeProvider.get(), _fileLoggingTarget.get()));
    Logger::current = std::move(_loggerFactory->create(ENCRYPT_STRING("iGP11")));
    log(ENCRYPT_STRING("iGP11 started"));

    _direct3D11PluginLoader = std::shared_ptr<core::IDirect3D11PluginLoader>(new Direct3D11PluginLoader(settings.direct3D11PluginPath, settings.pluginSettings, settings.direct3D11Settings, _loggerFactory.get()));
    _pluginLoaders.push_back(_direct3D11PluginLoader);

    auto pluginLoader = getPluginLoader();
    auto pluginName = pluginLoader->getName();
    log(core::stringFormat(ENCRYPT_STRING("attempting to load %s..."), pluginName.c_str()));

    if (::loadPlugin(pluginLoader.get())) {
        log(core::stringFormat(ENCRYPT_STRING("%s loaded"), pluginName.c_str()));
        log(core::stringFormat(ENCRYPT_STRING("attempting to start %s..."), pluginName.c_str()));

        if (::startPlugin(pluginLoader.get())) {
            log(core::stringFormat(ENCRYPT_STRING("%s started"), pluginName.c_str()));
        }
        else {
            log(error, core::stringFormat(ENCRYPT_STRING("%s failed to start"), pluginName.c_str()));
        }
    }
    else {
        log(error, core::stringFormat(ENCRYPT_STRING("%s failed to load, skipping..."), pluginName.c_str()));
    }

    ::initializeServer();

    return NULL;
}

BOOL APIENTRY DllMain(HINSTANCE module, DWORD reason, LPVOID reserved) {
    if (reason == DLL_PROCESS_ATTACH) {
        ::CreateThread(NULL, 0, ::initialize, NULL, 0, NULL);
    }
    else if (reason == DLL_PROCESS_DETACH) {
        ::deinitializeServer();

        auto pluginLoader = getPluginLoader();
        log(core::stringFormat(ENCRYPT_STRING("attempting to stop %s..."), pluginLoader->getName().c_str()));
        
        if (::stopPlugin(pluginLoader.get())) {
            log(core::stringFormat(ENCRYPT_STRING("%s stopped"), pluginLoader->getName().c_str()));
        }
        else {
            log(error, core::stringFormat(ENCRYPT_STRING("%s failed to stop"), pluginLoader->getName().c_str()));
        }

        log(ENCRYPT_STRING("iGP11 stopped"));
    }

    return TRUE;
}