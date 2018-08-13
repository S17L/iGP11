#include "stdafx.h"
#include "baseefect.h"

direct3d11::BaseEffect::BaseEffect(direct3d11::Direct3D11Context *context, direct3d11::dto::RenderingResolution resolution, direct3d11::ShaderCodeFactory *codeFactory)
    : _context(context), _resolution(resolution), _codeFactory(codeFactory) {}

void direct3d11::BaseEffect::begin() {
    if (!_init) {
        ThreadLoggerAppenderScope scope(core::stringFormat(ENCRYPT_STRING("%s: initialization"), getName().c_str()));
        _renderTarget.reset(new direct3d11::Renderer(_context, _resolution));
        _shaderApplicator.reset(new direct3d11::Pass(_context, getCode(_context, _codeFactory), _resolution));
        _init = true;
    }

    _renderTarget->begin();
    _shaderApplicator->begin();
}

void direct3d11::BaseEffect::render() {
    _renderTarget->render();
}

void direct3d11::BaseEffect::end() {
    _shaderApplicator->end();
    _renderTarget->end();
}