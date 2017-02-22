#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"
#include "render.h"
#include "utility.h"

using namespace core::logging;

namespace direct3d11 {
    class BaseEffect : public IEffect {
    private:
        bool _init = false;
        Direct3D11Context *_context;
        dto::RenderingResolution _resolution;
        ShaderCodeFactory *_codeFactory;
        std::unique_ptr<SquareRenderTarget> _renderTarget;
        std::unique_ptr<ShaderApplicator> _shaderApplicator;
    public:
        BaseEffect(Direct3D11Context *context, dto::RenderingResolution resolution, ShaderCodeFactory *codeFactory);
        virtual ~BaseEffect() {};
        virtual void begin() override final;
        virtual void render() override final;
        virtual void end() override final;
    protected:
        virtual std::string getName() = 0;
        virtual ShaderCode getCode(Direct3D11Context *context, ShaderCodeFactory *codeFactory) = 0;
    };
}