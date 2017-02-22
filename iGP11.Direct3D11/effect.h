#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"

#include "alpha.h"
#include "baseefect.h"
#include "bokehdof.h"
#include "codebuilder.h"
#include "depth.h"
#include "horizontalgaussianblur.h"
#include "lumasharpen.h"
#include "luminescence.h"
#include "render.h"
#include "tonemap.h"
#include "utility.h"
#include "verticalgaussianblur.h"
#include "vibrance.h"

using namespace core::logging;

namespace direct3d11 {
    namespace dto {
        struct FilterConfiguration {
            core::dto::BokehDoF bokehDoF;
            core::dto::DepthBuffer depthBuffer;
            core::dto::Direct3D11PluginSettings pluginSettings;
            core::dto::LumaSharpen lumaSharpen;
            core::dto::Tonemap tonemap;
            core::dto::Vibrance vibrance;
        };
    }
}

namespace direct3d11 {
	class EffectsApplicator {
	private:
		bool _hasError = false;
		bool _initRequested = false;
		dto::FilterConfiguration _configuration;
		dto::FilterConfiguration _requestedConfiguration;
		Direct3D11Context *_context;
		std::unique_ptr<ShaderCodeFactory> _codeBuilderFactory;
		std::unique_ptr<RenderingProxy> _proxy;
		std::list<std::shared_ptr<IEffect>> _effects;
        direct3d11::dto::RenderingResolution _resolution;
		ID3D11Texture2D *_currentColorTexture = nullptr;
        ID3D11Texture2D *_currentDepthTexture = nullptr;
		void addEffect(IEffect *effect);
		void applyProcessing(const dto::PostProcessingConfiguration &configuration);
        void clear();
	public:
		EffectsApplicator(
			dto::FilterConfiguration configuration,
			Direct3D11Context *context);
		void apply(const dto::PostProcessingConfiguration &configuration);
        void deinitialize();
        bool initializationRequired(const dto::PostProcessingConfiguration &configuration);
		void update(dto::FilterConfiguration configuration);
	};
}