#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace core {
	namespace communication {
		class GetActivationStatusCommandHandlingPolicy : public ICommandHandlingPolicy {
		private:
			std::shared_ptr<IPluginLoader> _pluginLoader;
		public:
			GetActivationStatusCommandHandlingPolicy(std::shared_ptr<IPluginLoader> pluginLoader)
				: _pluginLoader(pluginLoader) {}
			virtual ~GetActivationStatusCommandHandlingPolicy() {}
			virtual bool isAppliciable(const int &id) override;
			virtual std::string handle(const std::string &data) override;
		};

		class GetProxySettingsCommandHandlingPolicy : public ICommandHandlingPolicy {
		private:
            std::shared_ptr<IPluginLoader> _pluginLoader;
            IInjectionSettingsRepository *_repository;
			ISerializer *_serializer;
		public:
			GetProxySettingsCommandHandlingPolicy(
                std::shared_ptr<IPluginLoader> pluginLoader,
				IInjectionSettingsRepository *repository,
				ISerializer *serializer)
				:
                _pluginLoader(pluginLoader),
                _repository(repository),
				_serializer(serializer) {}
			virtual ~GetProxySettingsCommandHandlingPolicy() {}
			virtual bool isAppliciable(const int &id) override;
			virtual std::string handle(const std::string &data) override;
		};

		class UpdateProxySettingsCommandHandlingPolicy : public ICommandHandlingPolicy {
		private:
			std::shared_ptr<IDirect3D11PluginLoader> _direct3D11PluginLoader;
            IInjectionSettingsRepository *_repository;
			ISerializer *_serializer;
		public:
			UpdateProxySettingsCommandHandlingPolicy(
				std::shared_ptr<IDirect3D11PluginLoader> direct3D11PluginLoader,
                IInjectionSettingsRepository *repository,
				ISerializer *serializer)
				:
				_direct3D11PluginLoader(direct3D11PluginLoader),
                _repository(repository),
				_serializer(serializer) {}
			virtual ~UpdateProxySettingsCommandHandlingPolicy() {}
			virtual bool isAppliciable(const int &id) override;
			virtual std::string handle(const std::string &data) override;
		};
	}
}