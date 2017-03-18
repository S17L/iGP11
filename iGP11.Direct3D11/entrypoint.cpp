#pragma once

#include "stdafx.h"
#include "direct3d11plugin.h"
#include "hookservice.h"
#include "processservice.h"
#include "profilepicker.h"
#include "resourceprovider.h"
#include "jsonserializer.h"
#include "textureservice.h"

using namespace core::logging;

#define DLL __declspec(dllexport)

std::unique_ptr<core::IProcessService> _processService;
std::unique_ptr<core::ISerializer> _serializer;
std::unique_ptr<core::ITextureCacheFactory> _textureCacheFactory;
std::unique_ptr<direct3d11::IProfilePicker> _profilePicker;
std::unique_ptr<direct3d11::ITextureService> _textureService;

#ifdef __cplusplus
extern "C"
{
#endif

    DLL bool start(core::dto::PluginSettings pluginSettings, core::dto::Direct3D11Settings settings, core::logging::ILoggerFactory *loggerFactory) {
        Direct3D11Plugin &plugin = Direct3D11Plugin::getInstance();
        if (plugin.isInitialized()) {
            return false;
        }

        Logger::current = std::move(loggerFactory->create(ENCRYPT_STRING("iGP11.direct3d11")));

        auto init = plugin.initialize(
            &core::MinHookService::getInstance(),
            _processService.get(),
            _serializer.get(),
            _textureCacheFactory.get(),
            pluginSettings,
            settings,
            _profilePicker.get(),
            _textureService.get());

        return init && plugin.start();
    }

    DLL bool stop() {
        Direct3D11Plugin &plugin = Direct3D11Plugin::getInstance();
        if (!plugin.isInitialized()) {
            return false;
        }

        return plugin.deinitialize() && plugin.stop();
    }

    DLL bool get(core::IDirect3D11Plugin **plugin) {
        *plugin = &Direct3D11Plugin::getInstance();
        return *plugin != nullptr;
    }

#ifdef __cplusplus
}
#endif

BOOL APIENTRY DllMain(HINSTANCE module, DWORD reason, LPVOID reserved) {
    if (reason == DLL_PROCESS_ATTACH) {
        log(ENCRYPT_STRING("iGP11.Direct3D11.dll attached"));
        _processService.reset(new core::ProcessService());
        _serializer.reset(new core::JsonSerializer());
        _textureCacheFactory.reset(new core::TextureCacheFactory());
        _profilePicker.reset(new direct3d11::ProfilePicker());
        _textureService.reset(new direct3d11::TextureService());
    }
    else if (reason == DLL_PROCESS_DETACH) {
        log(ENCRYPT_STRING("iGP11.Direct3D11.dll detached"));
        _processService.reset();
        _serializer.reset();
        _textureCacheFactory.reset();
        _profilePicker.reset();
        _textureService.reset();
    }

    return TRUE;
}