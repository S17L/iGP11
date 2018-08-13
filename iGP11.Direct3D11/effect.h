#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"

#include "codebuilder.h"
#include "render.h"
#include "utility.h"

using namespace core::logging;

namespace direct3d11 {
    namespace dto {
        struct FilterSettings {
            std::list<core::dto::TechniqueData> techniques;
            std::string codeDirectoryPath;
            core::dto::DepthBuffer depthBuffer;
            core::dto::Direct3D11PluginSettings pluginSettings;
        };
    }
}

namespace direct3d11 {
    class Technique : public ITechnique {
    private:
        Direct3D11Context *_context;
        core::Technique _technique;
        direct3d11::ITexture *_color;
        ID3D11ShaderResourceView *_depth;
        direct3d11::ITexture *_output;
        std::map<core::type_tex_id, std::shared_ptr<ITexture>> _texturesById;
        std::map<core::type_tex_id, core::type_slot> _slotById;
        std::list<std::shared_ptr<PassSettings>> _passSettings;
        std::list<std::shared_ptr<Pass>> _passes;
        void init();
    public:
        Technique(
            Direct3D11Context *context,
            core::Technique technique,
            direct3d11::ITexture *color,
            ID3D11ShaderResourceView *depth,
            direct3d11::ITexture *output);
        virtual ~Technique() {};
        virtual void render() override final;
    };

	class TechniqueApplicator {
	private:
		bool _hasError = false;
		bool _initRequested = false;
		dto::FilterSettings _filterSettings;
		dto::FilterSettings _requestedFilterSettings;
		Direct3D11Context *_context;
        core::ISerializer *_serializer;
		std::unique_ptr<RenderingProxy> _proxy;
		std::list<std::shared_ptr<ITechnique>> _techniques;
        core::dto::Resolution _resolution;
		ID3D11Texture2D *_currentColorTexture = nullptr;
        ID3D11Texture2D *_currentDepthTexture = nullptr;
		void addTechnique(ITechnique *technique);
		void applyProcessing(const dto::PostProcessingSettings &postProcessingSettings);
        void clear();
	public:
		TechniqueApplicator(
			dto::FilterSettings filterSettings,
			Direct3D11Context *context,
            core::ISerializer *serializer);
		void apply(const dto::PostProcessingSettings &postProcessingSettings);
        void deinitialize();
        bool initializationRequired(const dto::PostProcessingSettings &postProcessingSettings);
		void update(dto::FilterSettings filterSettings);
	};
}