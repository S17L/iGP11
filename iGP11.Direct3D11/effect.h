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
#include "liftgammagain.h"
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
        struct FilterSettings {
            std::string codeDirectoryPath;
            core::dto::BokehDoF bokehDoF;
            core::dto::DepthBuffer depthBuffer;
            core::dto::Direct3D11PluginSettings pluginSettings;
            core::dto::LiftGammaGain liftGammaGain;
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
		dto::FilterSettings _filterSettings;
		dto::FilterSettings _requestedFilterSettings;
		Direct3D11Context *_context;
		std::unique_ptr<ShaderCodeFactory> _codeBuilderFactory;
		std::unique_ptr<RenderingProxy> _proxy;
		std::list<std::shared_ptr<IEffect>> _effects;
        direct3d11::dto::RenderingResolution _resolution;
		ID3D11Texture2D *_currentColorTexture = nullptr;
        ID3D11Texture2D *_currentDepthTexture = nullptr;
		void addEffect(IEffect *effect);
		void applyProcessing(const dto::PostProcessingSettings &postProcessingSettings);
        void clear();
	public:
		EffectsApplicator(
			dto::FilterSettings filterSettings,
			Direct3D11Context *context);
		void apply(const dto::PostProcessingSettings &postProcessingSettings);
        void deinitialize();
        bool initializationRequired(const dto::PostProcessingSettings &postProcessingSettings);
		void update(dto::FilterSettings filterSettings);
	};
}