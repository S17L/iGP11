#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace core {
	class JsonSerializer : public ISerializer {
	public:
		virtual ~JsonSerializer() {}
		virtual core::dto::Command deserializeCommand(const std::string &value) override;
		virtual core::dto::InjectionSettings deserializeSettings(const std::string &value) override;
		virtual core::dto::Direct3D11Settings deserializeDirect3D11Settings(const std::string &value) override;
        virtual core::dto::UpdateProxySettings deserializeUpdateProxySettings(const std::string &value) override;
		virtual std::string serialize(core::dto::ProxySettings data) override;
		virtual std::string serialize(core::dto::Direct3D11Settings data) override;
	};
}