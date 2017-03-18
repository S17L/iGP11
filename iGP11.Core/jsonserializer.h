#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace core {
	class JsonSerializer : public ISerializer {
	public:
		virtual ~JsonSerializer() {}
		virtual core::dto::Command deserializeCommand(const std::string &value) override;
        virtual core::dto::BokehDoF deserializeBokehDoF(const std::string &value) override;
        virtual core::dto::Denoise deserializeDenoise(const std::string &value) override;
        virtual core::dto::LiftGammaGain deserializeLiftGammaGain(const std::string &value) override;
        virtual core::dto::LumaSharpen deserializeLumaSharpen(const std::string &value) override;
        virtual core::dto::Tonemap deserializeTonemap(const std::string &value) override;
        virtual core::dto::Vibrance deserializeVibrance(const std::string &value) override;
		virtual core::dto::GameSettings deserializeSettings(const std::string &value) override;
		virtual core::dto::Direct3D11Settings deserializeDirect3D11Settings(const std::string &value) override;
        virtual core::dto::UpdateProxySettings deserializeUpdateProxySettings(const std::string &value) override;
		virtual std::string serialize(core::dto::ProxySettings data) override;
		virtual std::string serialize(core::dto::Direct3D11Settings data) override;
	};
}