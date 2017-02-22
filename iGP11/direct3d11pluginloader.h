#include "stdafx.h"
#include "igp11core.h"
#include "logger.h"

class Direct3D11PluginLoader : public core::IDirect3D11PluginLoader {
private:
	bool _hasError;
	HINSTANCE _library;
    std::string _pluginPath;
	core::dto::Direct3D11Settings _settings;
	core::logging::ILoggerFactory *_loggerFactory;
	void loadLibrary();
public:
	Direct3D11PluginLoader(std::string pluginPath, core::dto::Direct3D11Settings settings, core::logging::ILoggerFactory *loggerFactory)
        : _pluginPath(pluginPath), _hasError(false), _settings(settings), _loggerFactory(loggerFactory) {}
	virtual ~Direct3D11PluginLoader();
	virtual std::string getName() override;
	virtual core::PluginType getType() override;
	virtual bool load() override;
	virtual bool start() override;
	virtual bool stop() override;
	virtual core::IPlugin* getPlugin() override;
	virtual core::IDirect3D11Plugin* getDirect3D11Plugin() override;
};