#include "stdafx.h"
#include "bokehdof.h"

direct3d11::BokehDoFEffect::BokehDoFEffect(direct3d11::Direct3D11Context *context, direct3d11::ITexture *colorTexture, ID3D11ShaderResourceView *depthTextureView, direct3d11::dto::RenderingResolution resolution, core::dto::BokehDoF bokehDoF, core::dto::DepthBuffer depthBuffer, direct3d11::ShaderCodeFactory *codeFactory) {
    _context = context;
    _colorTexture = colorTexture;
    _depthTextureView = depthTextureView;
    _resolution = resolution;
    _bokehDoF = bokehDoF;
    _depthBuffer = depthBuffer;
    _codeFactory = codeFactory;
}

std::string direct3d11::BokehDoFEffect::getName() {
    return ENCRYPT_STRING("direct3d11::BokehDoFEffect");
}

void direct3d11::BokehDoFEffect::begin() {
    if (!_init) {
        ThreadLoggerAppenderScope scope(core::stringFormat(ENCRYPT_STRING("%s initialization"), getName().c_str()));
        auto textureDescription = direct3d11::utility::getDescription(_colorTexture->get());
        textureDescription = direct3d11::utility::createFloatTextureDescription(textureDescription);
        log(debug, core::stringFormat(ENCRYPT_STRING("color texture description: %s"), direct3d11::stringify::toString(&textureDescription).c_str()));

        auto gaussianBlurConfiguration = direct3d11::utility::getGaussianBlurConfiguration(_bokehDoF.blurStrength);
        auto shaderApplicatorFactory = [&](const direct3d11::ShaderCode &code) { return new direct3d11::ShaderApplicator(_context, code, _resolution); };

        _firstTexture.reset(new direct3d11::Texture(_context, textureDescription));
        _secondTexture.reset(new direct3d11::Texture(_context, textureDescription));
        _renderTarget.reset(new direct3d11::SquareRenderTarget(_context, _resolution));

        _cocShaderApplicator.reset(shaderApplicatorFactory(_codeFactory->createBokehDoFCoCCode(_colorTexture->getShaderView(), _depthTextureView, _bokehDoF, _depthBuffer)));
        _horizontalGaussianBlurShaderApplicator.reset(shaderApplicatorFactory(_codeFactory->createHorizontalGaussianBlurCode(_firstTexture->getShaderView(), gaussianBlurConfiguration.size, gaussianBlurConfiguration.sigma)));
        _verticalGaussianBlurShaderApplicator.reset(shaderApplicatorFactory(_codeFactory->createVerticalGaussianBlurCode(_secondTexture->getShaderView(), gaussianBlurConfiguration.size, gaussianBlurConfiguration.sigma)));
        _blurPassFirstShaderApplicator.reset(shaderApplicatorFactory(_codeFactory->createBokehDoFCode(_colorTexture->getShaderView(), _depthTextureView, _firstTexture->getShaderView(), core::BokehDoFPassType::first, _bokehDoF, _depthBuffer)));
        _blurPassSecondShaderApplicator.reset(shaderApplicatorFactory(_codeFactory->createBokehDoFCode(_colorTexture->getShaderView(), _depthTextureView, _secondTexture->getShaderView(), core::BokehDoFPassType::second, _bokehDoF, _depthBuffer)));
        _blurPassThirdShaderApplicator.reset(shaderApplicatorFactory(_codeFactory->createBokehDoFCode(_colorTexture->getShaderView(), _depthTextureView, _firstTexture->getShaderView(), core::BokehDoFPassType::third, _bokehDoF, _depthBuffer)));
        _chromaticAberrationShaderApplicator.reset(shaderApplicatorFactory(_codeFactory->createBokehDoFChromaticAberrationCode(_secondTexture->getShaderView(), _bokehDoF)));

        _blendingShaderApplicator.reset(shaderApplicatorFactory(_codeFactory->createBokehDoFBlendingCode(_colorTexture->getShaderView(), _depthTextureView, _firstTexture->getShaderView(), _bokehDoF, _depthBuffer)));
        _init = true;
    }

    _renderTarget->begin();
    _cocShaderApplicator->begin();
    _firstTexture->setAsRenderer();
    _renderTarget->render();

    if (_bokehDoF.isBlurEnabled) {
        direct3d11::utility::setNoRenderer(_context);
        _horizontalGaussianBlurShaderApplicator->begin();
        _secondTexture->setAsRenderer();
        _renderTarget->render();
        direct3d11::utility::setNoRenderer(_context);
        _verticalGaussianBlurShaderApplicator->begin();
        _firstTexture->setAsRenderer();
        _renderTarget->render();
    }

    direct3d11::utility::setNoRenderer(_context);
    _blurPassFirstShaderApplicator->begin();
    _secondTexture->setAsRenderer();
    _renderTarget->render();
    direct3d11::utility::setNoRenderer(_context);
    _blurPassSecondShaderApplicator->begin();
    _firstTexture->setAsRenderer();
    _renderTarget->render();
    direct3d11::utility::setNoRenderer(_context);
    _blurPassThirdShaderApplicator->begin();
    _secondTexture->setAsRenderer();
    _renderTarget->render();
    direct3d11::utility::setNoRenderer(_context);
    _chromaticAberrationShaderApplicator->begin();
    _firstTexture->setAsRenderer();
    _renderTarget->render();

    direct3d11::utility::setNoRenderer(_context);
    _blendingShaderApplicator->begin();
}

void direct3d11::BokehDoFEffect::render() {
    _renderTarget->render();
}

void direct3d11::BokehDoFEffect::end() {
    _blendingShaderApplicator->end();
    _chromaticAberrationShaderApplicator->end();

    _blurPassThirdShaderApplicator->end();
    _blurPassSecondShaderApplicator->end();
    _blurPassFirstShaderApplicator->end();

    if (_bokehDoF.isBlurEnabled) {
        _verticalGaussianBlurShaderApplicator->end();
        _horizontalGaussianBlurShaderApplicator->end();
    }

    _cocShaderApplicator->end();
    _renderTarget->end();
}