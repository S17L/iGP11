#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"

using namespace core::logging;

namespace direct3d11 {
    class DeviceContextOMBlendState : public IState {
    private:
        ID3D11DeviceContext *_deviceContext;
        ID3D11BlendState *_blendState;
        FLOAT _blendStateFactor[4] = {};
        UINT _blendStateSampleMask;
    public:
        DeviceContextOMBlendState(ID3D11DeviceContext *deviceContext)
            : _deviceContext(deviceContext) {}
        virtual void acquire() override;
        virtual void restore() override;
    };

    class DeviceContextOMDepthStencilState : public IState {
    private:
        ID3D11DeviceContext *_deviceContext;
        ID3D11DepthStencilState *_depthStencilState;
        UINT _stencil;
    public:
        DeviceContextOMDepthStencilState(ID3D11DeviceContext *deviceContext)
            : _deviceContext(deviceContext) {}
        virtual void acquire() override;
        virtual void restore() override;
    };

    class DeviceContextOMRenderTargets : public IState {
    private:
        ID3D11DeviceContext *_deviceContext;
        ID3D11RenderTargetView *_renderTargetViews[D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT] = {};
        ID3D11DepthStencilView *_depthStencilView;
    public:
        DeviceContextOMRenderTargets(ID3D11DeviceContext *deviceContext)
            : _deviceContext(deviceContext) {}
        virtual void acquire() override;
        virtual void restore() override;
    };

    class DeviceContextRSState : public IState {
    private:
        ID3D11DeviceContext *_deviceContext;
        ID3D11RasterizerState *_rasterizerState;
    public:
        DeviceContextRSState(ID3D11DeviceContext *deviceContext)
            : _deviceContext(deviceContext) {}
        virtual void acquire() override;
        virtual void restore() override;
    };

    class DeviceContextRSViewports : public IState {
    private:
        ID3D11DeviceContext *_deviceContext;
        D3D11_VIEWPORT _viewports[D3D11_VIEWPORT_AND_SCISSORRECT_OBJECT_COUNT_PER_PIPELINE] = {};
        UINT _viewportCount = D3D11_VIEWPORT_AND_SCISSORRECT_OBJECT_COUNT_PER_PIPELINE;
    public:
        DeviceContextRSViewports(ID3D11DeviceContext *deviceContext)
            : _deviceContext(deviceContext) {}
        virtual void acquire() override;
        virtual void restore() override;
    };

    class DeviceContextVSConstantBuffers : public IState {
    private:
        ID3D11DeviceContext *_deviceContext;
        ID3D11Buffer *_constantBuffers[D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT] = {};
    public:
        DeviceContextVSConstantBuffers(ID3D11DeviceContext *deviceContext)
            : _deviceContext(deviceContext) {}
        virtual void acquire() override;
        virtual void restore() override;
    };

    class DeviceContextPSShaderResources : public IState {
    private:
        ID3D11DeviceContext *_deviceContext;
        ID3D11ShaderResourceView *_shaderResourceViews[D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT] = {};
    public:
        DeviceContextPSShaderResources(ID3D11DeviceContext *deviceContext)
            : _deviceContext(deviceContext) {}
        virtual void acquire() override;
        virtual void restore() override;
    };

    class DeviceContextIAInputLayout : public IState {
    private:
        ID3D11DeviceContext *_deviceContext;
        ID3D11InputLayout *_inputLayer = nullptr;
    public:
        DeviceContextIAInputLayout(ID3D11DeviceContext *deviceContext)
            : _deviceContext(deviceContext) {}
        virtual void acquire() override;
        virtual void restore() override;
    };

    class DeviceContextVSShader : public IState {
    private:
        ID3D11DeviceContext *_deviceContext;
        ID3D11VertexShader *_vertexShader = nullptr;
        ID3D11ClassInstance *_vertexShaderClassInstances = nullptr;
        UINT _vertexShaderClassInstancesCount = 0;
    public:
        DeviceContextVSShader(ID3D11DeviceContext *deviceContext)
            : _deviceContext(deviceContext) {}
        virtual void acquire() override;
        virtual void restore() override;
    };

    class DeviceContextPSShader : public IState {
    private:
        ID3D11DeviceContext *_deviceContext;
        ID3D11PixelShader *_pixelShader = nullptr;
        ID3D11ClassInstance *_pixelShaderClassInstances = nullptr;
        UINT _pixelShaderClassInstancesCount = 0;
    public:
        DeviceContextPSShader(ID3D11DeviceContext *deviceContext)
            : _deviceContext(deviceContext) {}
        virtual void acquire() override;
        virtual void restore() override;
    };

    class DeviceContextPSSamplers : public IState {
    private:
        ID3D11DeviceContext *_deviceContext;
        ID3D11SamplerState *_samplerStates[D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT] = {};
    public:
        DeviceContextPSSamplers(ID3D11DeviceContext *deviceContext)
            : _deviceContext(deviceContext) {}
        virtual void acquire() override;
        virtual void restore() override;
    };

    class DeviceContextIAVertexBuffers : public IState {
    private:
        ID3D11DeviceContext *_deviceContext;
        ID3D11Buffer *_vertexBuffers[D3D11_IA_VERTEX_INPUT_RESOURCE_SLOT_COUNT] = {};
        UINT _vertexBufferStrides[D3D11_IA_VERTEX_INPUT_RESOURCE_SLOT_COUNT] = {};
        UINT _vertexBufferOffsets[D3D11_IA_VERTEX_INPUT_RESOURCE_SLOT_COUNT] = {};
    public:
        DeviceContextIAVertexBuffers(ID3D11DeviceContext *deviceContext)
            : _deviceContext(deviceContext) {}
        virtual void acquire() override;
        virtual void restore() override;
    };

    class DeviceContextIAIndexBuffer : public IState {
    private:
        ID3D11DeviceContext *_deviceContext;
        ID3D11Buffer *_indexBuffer;
        DXGI_FORMAT _indexBufferFormat;
        UINT _indexBufferOffset;
    public:
        DeviceContextIAIndexBuffer(ID3D11DeviceContext *deviceContext)
            : _deviceContext(deviceContext) {}
        virtual void acquire() override;
        virtual void restore() override;
    };

    class DeviceContextIAPrimitiveTopology : public IState {
    private:
        ID3D11DeviceContext *_deviceContext;
        D3D11_PRIMITIVE_TOPOLOGY _primitiveTopology;
    public:
        DeviceContextIAPrimitiveTopology(ID3D11DeviceContext *deviceContext)
            : _deviceContext(deviceContext) {}
        virtual void acquire() override;
        virtual void restore() override;
    };
}