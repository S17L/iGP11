#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"
#include "render.h"
#include "utility.h"

namespace direct3d11 {
    class BokehDoFEffect final : public IEffect {
    private:
        bool _init = false;
        Direct3D11Context *_context;
        ShaderCodeFactory *_codeFactory;
        std::unique_ptr<ITexture> _firstTexture;
        std::unique_ptr<ITexture> _secondTexture;
        std::unique_ptr<Renderer> _renderTarget;
        std::unique_ptr<Pass> _cocShaderApplicator;
        std::unique_ptr<Pass> _blurPassFirstShaderApplicator;
        std::unique_ptr<Pass> _blurPassSecondShaderApplicator;
        std::unique_ptr<Pass> _blurPassThirdShaderApplicator;
        std::unique_ptr<Pass> _chromaticAberrationShaderApplicator;
        std::unique_ptr<Pass> _horizontalGaussianBlurShaderApplicator;
        std::unique_ptr<Pass> _verticalGaussianBlurShaderApplicator;
        std::unique_ptr<Pass> _blendingShaderApplicator;
        ITexture *_colorTexture;
        ID3D11ShaderResourceView *_depthTextureView;
        dto::RenderingResolution _resolution;
        core::dto::BokehDoF _bokehDoF;
        core::dto::DepthBuffer _depthBuffer;
    public:
        BokehDoFEffect(Direct3D11Context *context, ITexture *colorTexture, ID3D11ShaderResourceView *depthTextureView, dto::RenderingResolution resolution, core::dto::BokehDoF bokehDoF, core::dto::DepthBuffer depthBuffer, ShaderCodeFactory *codeFactory);
        virtual ~BokehDoFEffect() {};
        virtual std::string getName() override;
        virtual void begin() override;
        virtual void render() override;
        virtual void end() override;
    };
}