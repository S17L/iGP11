#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"
#include "baseefect.h"

namespace direct3d11 {
    class AlphaEffect : public BaseEffect {
    private:
        ITexture *_colorTexture;
    public:
        AlphaEffect(Direct3D11Context *context, ITexture *colorTexture, dto::RenderingResolution resolution, ShaderCodeFactory *codeFactory);
        virtual ~AlphaEffect() {}
    protected:
        virtual std::string getName() override;
        virtual ShaderCode getCode(Direct3D11Context *context, ShaderCodeFactory *codeFactory) override;
    };
}