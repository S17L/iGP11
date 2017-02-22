#include "stdafx.h"
#include "direct3d11pluginloader.h"

typedef bool(__stdcall *startPluginDefinition)(core::dto::Direct3D11Settings settings, core::logging::ILoggerFactory *loggerFactory);
typedef bool(__stdcall *stopPluginDefinition)();
typedef bool(__stdcall *getPluginDefinition)(core::IDirect3D11Plugin **plugin);

Direct3D11PluginLoader::~Direct3D11PluginLoader() {
	if (_library) {
		::FreeLibrary(_library);
	}
}

void Direct3D11PluginLoader::loadLibrary() {
	_library = LoadLibrary(core::toWString(_pluginPath).c_str());
}

std::string Direct3D11PluginLoader::getName() {
	return ENCRYPT_STRING("iGP11.direct3d11");
}

core::PluginType Direct3D11PluginLoader::getType() {
	return core::PluginType::direct3d11;
}

bool Direct3D11PluginLoader::load() {
	if (_hasError) {
		return false;
	} else if (_library != nullptr) {
		return true;
	}

	__try {
		loadLibrary();
	} __except (EXCEPTION_EXECUTE_HANDLER) {
		_hasError = true;
	}

	return !_hasError && _library != nullptr;
}

bool Direct3D11PluginLoader::start() {
	if (_library) {
		auto function = (startPluginDefinition)::GetProcAddress(_library, ENCRYPT_STRING("start"));
		if (function) {
			return function(_settings, _loggerFactory);
		}
	}

	return false;
}

bool Direct3D11PluginLoader::stop() {
	if (_library) {
		auto function = (stopPluginDefinition)::GetProcAddress(_library, ENCRYPT_STRING("stop"));
		if (function) {
			return function();
		}
	}

	return false;
}

core::IPlugin* Direct3D11PluginLoader::getPlugin() {
	return getDirect3D11Plugin();
}

core::IDirect3D11Plugin* Direct3D11PluginLoader::getDirect3D11Plugin() {
	if (_library) {
		auto function = (getPluginDefinition)::GetProcAddress(_library, ENCRYPT_STRING("get"));
		if (function) {
			core::IDirect3D11Plugin *plugin = nullptr;
			return function(&plugin) ? plugin : nullptr;
		}
	}

	return nullptr;
}