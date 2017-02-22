#include "stdafx.h"
#include "commandhandlingpolicy.h"

static std::string translate(core::ActivationStatus activationStatus) {
    return core::stringFormat("%d", static_cast<int>(activationStatus));
}

bool core::communication::GetActivationStatusCommandHandlingPolicy::isAppliciable(const int &id) {
	return id == static_cast<int>(core::RequestType::getactivationstatus);
}

std::string core::communication::GetActivationStatusCommandHandlingPolicy::handle(const std::string &data) {
	auto plugin = _pluginLoader->getPlugin();
	if (plugin == nullptr) {
		return ::translate(core::ActivationStatus::running);
	}

	return ::translate(plugin->getActivationStatus());
}

bool core::communication::GetProxySettingsCommandHandlingPolicy::isAppliciable(const int &id) {
	return id == static_cast<int>(core::RequestType::getproxysettings);
}

std::string core::communication::GetProxySettingsCommandHandlingPolicy::handle(const std::string &data) {
	core::dto::ProxySettings state;
    core::dto::InjectionSettings settings = _repository->load();
    state.applicationFilePath = settings.applicationFilePath;
    state.configurationDirectoryPath = settings.configurationDirectoryPath;
    state.logsDirectoryPath = settings.logsDirectoryPath;
    state.direct3D11Settings = settings.direct3D11Settings;

    auto plugin = _pluginLoader->getPlugin();
    state.pluginType = _pluginLoader->getType();
    state.activationStatus = plugin->getActivationStatus();

	return _serializer->serialize(state);
}

bool core::communication::UpdateProxySettingsCommandHandlingPolicy::isAppliciable(const int &id) {
	return id == static_cast<int>(core::RequestType::updateproxysettings);
}

std::string core::communication::UpdateProxySettingsCommandHandlingPolicy::handle(const std::string &data) {
	bool result = false;
    auto settings = _serializer->deserializeUpdateProxySettings(data);
    if (settings.pluginType == core::PluginType::direct3d11) {
        auto plugin = _direct3D11PluginLoader->getDirect3D11Plugin();
        if (plugin != nullptr) {
            result = plugin->update(settings.direct3D11Settings);
            if (result) {
                _repository->update(settings.direct3D11Settings);
            }
        }
    }

	return core::stringFormat("%d", result);
}