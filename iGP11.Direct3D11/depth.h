#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"
#include "baseefect.h"

namespace direct3d11 {
    class DepthEffect : public BaseEffect {
    private:
        ITexture *_colorTexture;
        ID3D11ShaderResourceView *_depthTextureView;
        core::dto::DepthBuffer _depthBuffer;
    public:
        DepthEffect(Direct3D11Context *context, ITexture *colorTexture, ID3D11ShaderResourceView *depthTextureView, dto::RenderingResolution resolution, core::dto::DepthBuffer depthBuffer, ShaderCodeFactory *codeFactory);
        virtual ~DepthEffect() {}
    protected:
        virtual std::string getName() override;
        virtual PassSettings getCode(Direct3D11Context *context, ShaderCodeFactory *codeFactory) override;
    };
}