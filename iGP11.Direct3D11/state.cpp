#include "stdafx.h"
#include "state.h"

void direct3d11::DeviceContextOMBlendState::acquire() {
    _deviceContext->OMGetBlendState(&_blendState, _blendStateFactor, &_blendStateSampleMask);
}

void direct3d11::DeviceContextOMBlendState::restore() {
    _deviceContext->OMSetBlendState(_blendState, _blendStateFactor, _blendStateSampleMask);
    SAFE_RELEASE(_blendState);
}

void direct3d11::DeviceContextOMDepthStencilState::acquire() {
    _deviceContext->OMGetDepthStencilState(&_depthStencilState, &_stencil);
}

void direct3d11::DeviceContextOMDepthStencilState::restore() {
    _deviceContext->OMSetDepthStencilState(_depthStencilState, _stencil);
    SAFE_RELEASE(_depthStencilState);
}

void direct3d11::DeviceContextOMRenderTargets::acquire() {
    _deviceContext->OMGetRenderTargets(D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT, _renderTargetViews, &_depthStencilView);
}

void direct3d11::DeviceContextOMRenderTargets::restore() {
    _deviceContext->OMSetRenderTargets(D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT, _renderTargetViews, _depthStencilView);
    for (auto i = 0; i < D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT; i++) {
        SAFE_RELEASE_ARRAY(_renderTargetViews, i);
    }

    SAFE_RELEASE(_depthStencilView);
}

void direct3d11::DeviceContextRSState::acquire() {
    _deviceContext->RSGetState(&_rasterizerState);
}

void direct3d11::DeviceContextRSState::restore() {
    _deviceContext->RSSetState(_rasterizerState);
    SAFE_RELEASE(_rasterizerState);
}

void direct3d11::DeviceContextRSViewports::acquire() {
    _deviceContext->RSGetViewports(&_viewportCount, _viewports);
}

void direct3d11::DeviceContextRSViewports::restore() {
    _deviceContext->RSSetViewports(_viewportCount, _viewports);
}

void direct3d11::DeviceContextVSConstantBuffers::acquire() {
    _deviceContext->VSGetConstantBuffers(0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT, _constantBuffers);
}

void direct3d11::DeviceContextVSConstantBuffers::restore() {
    _deviceContext->VSSetConstantBuffers(0, D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT, _constantBuffers);
    for (auto i = 0; i < D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT; i++) {
        SAFE_RELEASE_ARRAY(_constantBuffers, i);
    }
}

void direct3d11::DeviceContextPSShaderResources::acquire() {
    _deviceContext->PSGetShaderResources(0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT, _shaderResourceViews);
}

void direct3d11::DeviceContextPSShaderResources::restore() {
    _deviceContext->PSSetShaderResources(0, D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT, _shaderResourceViews);
    for (auto i = 0; i < D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT; i++) {
        SAFE_RELEASE_ARRAY(_shaderResourceViews, i);
    }
}

void direct3d11::DeviceContextIAInputLayout::acquire() {
    _deviceContext->IAGetInputLayout(&_inputLayer);
}

void direct3d11::DeviceContextIAInputLayout::restore() {
    _deviceContext->IASetInputLayout(_inputLayer);
    SAFE_RELEASE(_inputLayer);
}

void direct3d11::DeviceContextVSShader::acquire() {
    _deviceContext->VSGetShader(&_vertexShader, &_vertexShaderClassInstances, &_vertexShaderClassInstancesCount);
}

void direct3d11::DeviceContextVSShader::restore() {
    _deviceContext->VSSetShader(_vertexShader, &_vertexShaderClassInstances, _vertexShaderClassInstancesCount);
    
    SAFE_RELEASE(_vertexShader);
    for (UINT i = 0; i < _vertexShaderClassInstancesCount; i++) {
        _vertexShaderClassInstances[i].Release();
    }

    _vertexShaderClassInstances = nullptr;
    _vertexShaderClassInstancesCount = 0;
}

void direct3d11::DeviceContextPSShader::acquire() {
    _deviceContext->PSGetShader(&_pixelShader, &_pixelShaderClassInstances, &_pixelShaderClassInstancesCount);
}

void direct3d11::DeviceContextPSShader::restore() {
    _deviceContext->PSSetShader(_pixelShader, &_pixelShaderClassInstances, _pixelShaderClassInstancesCount);
    
    SAFE_RELEASE(_pixelShader);
    for (UINT i = 0; i < _pixelShaderClassInstancesCount; i++) {
        _pixelShaderClassInstances[i].Release();
    }

    _pixelShaderClassInstances = nullptr;
    _pixelShaderClassInstancesCount = 0;
}

void direct3d11::DeviceContextPSSamplers::acquire() {
    _deviceContext->PSGetSamplers(0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT, _samplerStates);
}

void direct3d11::DeviceContextPSSamplers::restore() {
    _deviceContext->PSSetSamplers(0, D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT, _samplerStates);
    for (auto i = 0; i < D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT; i++) {
        SAFE_RELEASE_ARRAY(_samplerStates, i);
    }
}

void direct3d11::DeviceContextIAVertexBuffers::acquire() {
    _deviceContext->IAGetVertexBuffers(0, D3D11_IA_VERTEX_INPUT_RESOURCE_SLOT_COUNT, _vertexBuffers, _vertexBufferStrides, _vertexBufferOffsets);
}

void direct3d11::DeviceContextIAVertexBuffers::restore() {
    _deviceContext->IASetVertexBuffers(0, D3D11_IA_VERTEX_INPUT_RESOURCE_SLOT_COUNT, _vertexBuffers, _vertexBufferStrides, _vertexBufferOffsets);
    for (auto i = 0; i < D3D11_IA_VERTEX_INPUT_RESOURCE_SLOT_COUNT; i++) {
        SAFE_RELEASE_ARRAY(_vertexBuffers, i);
    }
}

void direct3d11::DeviceContextIAIndexBuffer::acquire() {
    _deviceContext->IAGetIndexBuffer(&_indexBuffer, &_indexBufferFormat, &_indexBufferOffset);
}

void direct3d11::DeviceContextIAIndexBuffer::restore() {
    _deviceContext->IASetIndexBuffer(_indexBuffer, _indexBufferFormat, _indexBufferOffset);
    SAFE_RELEASE(_indexBuffer);
}

void direct3d11::DeviceContextIAPrimitiveTopology::acquire() {
    _deviceContext->IAGetPrimitiveTopology(&_primitiveTopology);
}

void direct3d11::DeviceContextIAPrimitiveTopology::restore() {
    _deviceContext->IASetPrimitiveTopology(_primitiveTopology);
}