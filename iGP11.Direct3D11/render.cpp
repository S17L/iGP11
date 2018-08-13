#include "stdafx.h"
#include "render.h"

ID3D11SamplerState* createSampler(ID3D11Device *device, D3D11_SAMPLER_DESC description) {
    ID3D11SamplerState *sampler;
    auto result = device->CreateSamplerState(&description, &sampler);
    if (FAILED(result)) {
        throw core::exception::OperationException(ENCRYPT_STRING("::createSampler"), ENCRYPT_STRING("sampler creation failed"));
    }

    return sampler;
}

direct3d11::Texture::Texture(direct3d11::Direct3D11Context *context, const D3D11_TEXTURE2D_DESC &description) {
    _context = context;

    _texture = core::disposing::makeUnknown<ID3D11Texture2D>(nullptr);
    _textureShaderView = core::disposing::makeUnknown<ID3D11ShaderResourceView>(nullptr);
    _textureRenderView = core::disposing::makeUnknown<ID3D11RenderTargetView>(nullptr);

    ID3D11Texture2D *texture = nullptr;
    ID3D11ShaderResourceView *shaderResourceView = nullptr;
    ID3D11RenderTargetView *renderTargetView = nullptr;

    auto textureDescription = description;
    textureDescription.Usage = D3D11_USAGE_DEFAULT;
    textureDescription.BindFlags |= D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_RENDER_TARGET;

    auto device = _context->getDevice();
    auto result = device->CreateTexture2D(&textureDescription, nullptr, &texture);
    if (FAILED(result)) {
        throw core::exception::InitializationException(ENCRYPT_STRING("direct3d11::Texture"), ENCRYPT_STRING("texture could not be created"));
    }

    _texture.reset(texture);

    result = device->CreateShaderResourceView(_texture.get(), nullptr, &shaderResourceView);
    if (FAILED(result)) {
        throw core::exception::InitializationException(ENCRYPT_STRING("direct3d11::Texture"), ENCRYPT_STRING("texture shader view could not be created"));
    }

    _textureShaderView.reset(shaderResourceView);

    result = device->CreateRenderTargetView(_texture.get(), NULL, &renderTargetView);
    if (FAILED(result)) {
        throw core::exception::OperationException(ENCRYPT_STRING("direct3d11::Texture"), ENCRYPT_STRING("texture render view could not be created"));
    }

    _textureRenderView.reset(renderTargetView);
}

direct3d11::RenderingProxy::RenderingProxy(
    direct3d11::Direct3D11Context *context,
    ID3D11Texture2D *color,
    ID3D11Texture2D *depth) {
    ThreadLoggerAppenderScope scope(debug, ENCRYPT_STRING("initializing direct3d11::RenderingProxy"));
    log(debug, core::stringFormat("color: %p, depth: %p", color, depth));

    _context = context;
    _color = color;
    _depth = depth;

    _inputRenderTargetView = core::disposing::makeUnknown<ID3D11RenderTargetView>(nullptr);
    _depthTexture = core::disposing::makeUnknown<ID3D11Texture2D>(nullptr);
    _depthTextureView = core::disposing::makeUnknown<ID3D11ShaderResourceView>(nullptr);
    _rasterizerState = core::disposing::makeUnknown<ID3D11RasterizerState>(nullptr);
    _depthStencilState = core::disposing::makeUnknown<ID3D11DepthStencilState>(nullptr);

    ID3D11Texture2D *texture = nullptr;
    ID3D11ShaderResourceView *shaderResourceView = nullptr;
    ID3D11RenderTargetView *renderTargetView = nullptr;
    ID3D11RasterizerState *rasterizedState = nullptr;
    ID3D11DepthStencilState *depthStencilState = nullptr;

    auto device = _context->getDevice();
    auto textureDescription = direct3d11::utility::getDescription(_color);
    log(debug, core::stringFormat(ENCRYPT_STRING("color: %s"), direct3d11::stringify::toString(&textureDescription).c_str()));

    _colorTexture.reset(new direct3d11::Texture(_context, textureDescription));
    _chainFirstTexture.reset(new direct3d11::Texture(_context, direct3d11::utility::createFloatTextureDescription(textureDescription)));
    _chainSecondTexture.reset(new direct3d11::Texture(_context, direct3d11::utility::createFloatTextureDescription(textureDescription)));

    auto result = device->CreateRenderTargetView(_color, NULL, &renderTargetView);
    if (FAILED(result)) {
        throw core::exception::OperationException(ENCRYPT_STRING("direct3d11::RenderingProxy"), ENCRYPT_STRING("color render target could not be created"));
    }

    _inputRenderTargetView.reset(renderTargetView);
    _renderingTexture.reset(new direct3d11::CustomizableTexture(
        _context,
        _colorTexture->get(),
        _colorTexture->getInView(),
        _inputRenderTargetView.get()));

    if (_depth == nullptr) {
        return;
    }

    auto depthTextureDescription = direct3d11::utility::getDescription(_depth);
    log(debug, core::stringFormat(ENCRYPT_STRING("depth: %s"), direct3d11::stringify::toString(&depthTextureDescription).c_str()));
    depthTextureDescription.BindFlags |= D3D11_BIND_SHADER_RESOURCE;

    result = device->CreateTexture2D(&depthTextureDescription, nullptr, &texture);
    if (FAILED(result)) {
        throw core::exception::InitializationException(ENCRYPT_STRING("direct3d11::RenderingProxy"), ENCRYPT_STRING("depth texture could not be created"));
    }

    _depthTexture.reset(texture);

    D3D11_SHADER_RESOURCE_VIEW_DESC shaderResourceViewDescription;
    ZeroMemory(&shaderResourceViewDescription, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
    shaderResourceViewDescription.Format = direct3d11::utility::getDepthTextureViewFormat(depthTextureDescription.Format);
    shaderResourceViewDescription.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
    shaderResourceViewDescription.Texture2D.MipLevels = 1;
    shaderResourceViewDescription.Texture2D.MostDetailedMip = 0;

    result = device->CreateShaderResourceView(_depthTexture.get(), &shaderResourceViewDescription, &shaderResourceView);
    if (FAILED(result)) {
        throw core::exception::InitializationException(ENCRYPT_STRING("direct3d11::RenderingProxy"), ENCRYPT_STRING("depth texture view could not be created"));
    }

    _depthTextureView.reset(shaderResourceView);

    D3D11_RASTERIZER_DESC rasterizerDescription;
    rasterizerDescription.AntialiasedLineEnable = false;
    rasterizerDescription.CullMode = D3D11_CULL_BACK;
    rasterizerDescription.DepthBias = 0;
    rasterizerDescription.DepthBiasClamp = 0.0f;
    rasterizerDescription.DepthClipEnable = true;
    rasterizerDescription.FillMode = D3D11_FILL_SOLID;
    rasterizerDescription.FrontCounterClockwise = false;
    rasterizerDescription.MultisampleEnable = false;
    rasterizerDescription.ScissorEnable = false;
    rasterizerDescription.SlopeScaledDepthBias = 0.0f;

    result = device->CreateRasterizerState(&rasterizerDescription, &rasterizedState);
    if (FAILED(result)) {
        throw core::exception::InitializationException(
            ENCRYPT_STRING("direct3d11::RenderingProxy"),
            ENCRYPT_STRING("rasterized state could not be created"));
    }

    _rasterizerState.reset(rasterizedState);

    D3D11_DEPTH_STENCIL_DESC depthStencilDescription;
    depthStencilDescription.DepthEnable = false;
    depthStencilDescription.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ALL;
    depthStencilDescription.DepthFunc = D3D11_COMPARISON_LESS;
    depthStencilDescription.StencilEnable = true;
    depthStencilDescription.StencilReadMask = 0xFF;
    depthStencilDescription.StencilWriteMask = 0xFF;
    depthStencilDescription.FrontFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
    depthStencilDescription.FrontFace.StencilDepthFailOp = D3D11_STENCIL_OP_INCR;
    depthStencilDescription.FrontFace.StencilPassOp = D3D11_STENCIL_OP_KEEP;
    depthStencilDescription.FrontFace.StencilFunc = D3D11_COMPARISON_ALWAYS;
    depthStencilDescription.BackFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
    depthStencilDescription.BackFace.StencilDepthFailOp = D3D11_STENCIL_OP_DECR;
    depthStencilDescription.BackFace.StencilPassOp = D3D11_STENCIL_OP_KEEP;
    depthStencilDescription.BackFace.StencilFunc = D3D11_COMPARISON_ALWAYS;

    result = device->CreateDepthStencilState(&depthStencilDescription, &depthStencilState);
    if (FAILED(result)) {
        throw core::exception::InitializationException(
            ENCRYPT_STRING("direct3d11::RenderingProxy"),
            ENCRYPT_STRING("depth stencil state could not be created"));
    }

    _depthStencilState.reset(depthStencilState);

    auto deviceContext = _context->getDeviceContext();
    _states.push_back(std::shared_ptr<direct3d11::IState>(new direct3d11::DeviceContextOMBlendState(deviceContext.get())));
    _states.push_back(std::shared_ptr<direct3d11::IState>(new direct3d11::DeviceContextOMDepthStencilState(deviceContext.get())));
    _states.push_back(std::shared_ptr<direct3d11::IState>(new direct3d11::DeviceContextOMRenderTargets(deviceContext.get())));
    _states.push_back(std::shared_ptr<direct3d11::IState>(new direct3d11::DeviceContextRSState(deviceContext.get())));
}

void direct3d11::RenderingProxy::begin() {
    for (auto state : _states) {
        state->acquire();
    }

    auto deviceContext = _context->getDeviceContext();
    deviceContext->CopyResource(_colorTexture->get(), _color);
    if (_depth != nullptr) {
        deviceContext->CopyResource(_depthTexture.get(), _depth);
    }

    deviceContext->OMSetBlendState(NULL, NULL, 0xffffffff);
    deviceContext->OMSetDepthStencilState(_depthStencilState.get(), 1);
    deviceContext->RSSetState(_rasterizerState.get());
}

void direct3d11::RenderingProxy::end() {
    for (auto state : _states) {
        state->restore();
    }
}

direct3d11::ITexture* direct3d11::RenderingProxy::iterateIn() {
    _count++;
    if (_count == 1) {
        return _renderingTexture.get();
    }
    else if (_count % 2 == 0) {
        return _chainFirstTexture.get();
    }
    else {
        return _chainSecondTexture.get();
    }
}

direct3d11::ITexture* direct3d11::RenderingProxy::iterateOut(bool last) {
    if (last) {
        return _renderingTexture.get();
    }
    
    _index++;
    if (_index % 2 == 1) {
        return _chainFirstTexture.get();
    }
    else {
        return _chainSecondTexture.get();
    }
}

ID3D11ShaderResourceView* direct3d11::RenderingProxy::getDepthTextureView() const {
    return _depthTextureView.get();
}

direct3d11::Renderer::Renderer(
    direct3d11::Direct3D11Context *context,
    core::dto::Resolution resolution) {
    _context = context;
    _resolution = resolution;

    _count = 6;
    _indexBuffer = core::disposing::makeUnknown<ID3D11Buffer>(nullptr);
    _vertexBuffer = core::disposing::makeUnknown<ID3D11Buffer>(nullptr);

    auto device = _context->getDevice();
    auto deviceContext = _context->getDeviceContext();
    std::unique_ptr<direct3d11::dto::VertexType[]> vertices(new direct3d11::dto::VertexType[_count]);
    std::unique_ptr<unsigned long[]> indices(new unsigned long[_count]);

    ::memset(vertices.get(), 0, (sizeof(direct3d11::dto::VertexType) * _count));
    for (int i = 0; i < _count; i++) {
        indices[i] = i;
    }

    D3D11_BUFFER_DESC vertexBufferDescription;
    vertexBufferDescription.Usage = D3D11_USAGE_DYNAMIC;
    vertexBufferDescription.ByteWidth = sizeof(direct3d11::dto::VertexType) * _count;
    vertexBufferDescription.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    vertexBufferDescription.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    vertexBufferDescription.MiscFlags = 0;
    vertexBufferDescription.StructureByteStride = 0;

    D3D11_SUBRESOURCE_DATA vertexData;
    vertexData.pSysMem = vertices.get();
    vertexData.SysMemPitch = 0;
    vertexData.SysMemSlicePitch = 0;

    ID3D11Buffer *vertexBuffer = nullptr;
    auto result = device->CreateBuffer(&vertexBufferDescription, &vertexData, &vertexBuffer);
    if (FAILED(result)) {
        throw core::exception::InitializationException(
            ENCRYPT_STRING("direct3d11::Renderer"),
            ENCRYPT_STRING("vertex buffer could not be created"));
    }

    _vertexBuffer.reset(vertexBuffer);
    D3D11_BUFFER_DESC indexBufferDescription;
    indexBufferDescription.Usage = D3D11_USAGE_DEFAULT;
    indexBufferDescription.ByteWidth = sizeof(unsigned long) * _count;
    indexBufferDescription.BindFlags = D3D11_BIND_INDEX_BUFFER;
    indexBufferDescription.CPUAccessFlags = 0;
    indexBufferDescription.MiscFlags = 0;
    indexBufferDescription.StructureByteStride = 0;

    D3D11_SUBRESOURCE_DATA indexData;
    indexData.pSysMem = indices.get();
    indexData.SysMemPitch = 0;
    indexData.SysMemSlicePitch = 0;

    ID3D11Buffer *indexBuffer = nullptr;
    result = device->CreateBuffer(&indexBufferDescription, &indexData, &indexBuffer);
    if (FAILED(result)) {
        throw core::exception::InitializationException(
            ENCRYPT_STRING("direct3d11::Renderer"),
            ENCRYPT_STRING("index buffer could not be created"));
    }

    _indexBuffer.reset(indexBuffer);

    _states.push_back(std::shared_ptr<direct3d11::IState>(new direct3d11::DeviceContextIAVertexBuffers(deviceContext.get())));
    _states.push_back(std::shared_ptr<direct3d11::IState>(new direct3d11::DeviceContextIAIndexBuffer(deviceContext.get())));
    _states.push_back(std::shared_ptr<direct3d11::IState>(new direct3d11::DeviceContextIAPrimitiveTopology(deviceContext.get())));
}

void direct3d11::Renderer::render() {
    auto deviceContext = _context->getDeviceContext();
    deviceContext->DrawIndexed(_count, 0, 0);
}

void direct3d11::Renderer::begin() {
    float left = ((float)(_resolution.width / 2) * -1) + (float)_positionX;
    float right = left + (float)_resolution.width;
    float top = (float)(_resolution.height / 2) - (float)_positionY;
    float bottom = top - (float)_resolution.height;

    std::unique_ptr<direct3d11::dto::VertexType[]> vertices(new direct3d11::dto::VertexType[_count]);

    vertices[0].position = DirectX::XMFLOAT3(left, top, 0.0f);
    vertices[0].texture = DirectX::XMFLOAT2(0.0f, 0.0f);

    vertices[1].position = DirectX::XMFLOAT3(right, bottom, 0.0f);
    vertices[1].texture = DirectX::XMFLOAT2(1.0f, 1.0f);

    vertices[2].position = DirectX::XMFLOAT3(left, bottom, 0.0f);
    vertices[2].texture = DirectX::XMFLOAT2(0.0f, 1.0f);

    vertices[3].position = DirectX::XMFLOAT3(left, top, 0.0f);
    vertices[3].texture = DirectX::XMFLOAT2(0.0f, 0.0f);

    vertices[4].position = DirectX::XMFLOAT3(right, top, 0.0f);
    vertices[4].texture = DirectX::XMFLOAT2(1.0f, 0.0f);

    vertices[5].position = DirectX::XMFLOAT3(right, bottom, 0.0f);
    vertices[5].texture = DirectX::XMFLOAT2(1.0f, 1.0f);

    auto deviceContext = _context->getDeviceContext();
    auto vertexBuffer = _vertexBuffer.get();
    auto indexBuffer = _indexBuffer.get();

    D3D11_MAPPED_SUBRESOURCE resource;
    auto result = deviceContext->Map(vertexBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &resource);
    if (FAILED(result)) {
        throw core::exception::OperationException(
            ENCRYPT_STRING("direct3d11::Renderer"),
            ENCRYPT_STRING("resource could not be mapped"));
    }

    ::memcpy((direct3d11::dto::VertexType*)resource.pData, (void*)vertices.get(), (sizeof(direct3d11::dto::VertexType) * _count));
    deviceContext->Unmap(vertexBuffer, 0);

    for (auto state : _states) {
        state->acquire();
    }

    unsigned int stride = sizeof(direct3d11::dto::VertexType);
    unsigned int offset = 0;

    deviceContext->IASetVertexBuffers(0, 1, &vertexBuffer, &stride, &offset);
    deviceContext->IASetIndexBuffer(indexBuffer, DXGI_FORMAT_R32_UINT, 0);
    deviceContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
}

void direct3d11::Renderer::end() {
    for (auto state : _states) {
        state->restore();
    }
}

direct3d11::Pass::Pass(
    direct3d11::Direct3D11Context *context,
    PassSettings *passSettings,
    core::dto::Resolution resolution) {
    _context = context;
    _passSettings = passSettings;
    _resolution = resolution;

    DirectX::XMMATRIX world;
    DirectX::XMMATRIX view;
    DirectX::XMMATRIX projection;

    direct3d11::camera::getWorld(world);
    direct3d11::camera::getCamera(view, 0, 0, -10);
    direct3d11::camera::getOrtho(projection, (float)_resolution.width, (float)_resolution.height, 0.1f, 1000.0f);

    DirectX::XMStoreFloat4x4(&_matrixBufferType.world, world);
    DirectX::XMStoreFloat4x4(&_matrixBufferType.view, view);
    DirectX::XMStoreFloat4x4(&_matrixBufferType.projection, projection);

    _renderer.reset(new direct3d11::Renderer(_context, _resolution));
    _vertexShader = core::disposing::makeUnknown<ID3D11VertexShader>(nullptr);
    _pixelShader = core::disposing::makeUnknown<ID3D11PixelShader>(nullptr);
    _inputLayout = core::disposing::makeUnknown<ID3D11InputLayout>(nullptr);
    _matrixBuffer = core::disposing::makeUnknown<ID3D11Buffer>(nullptr);
    _pointSampler = core::disposing::makeUnknown<ID3D11SamplerState>(nullptr);
    _bilinearSampler = core::disposing::makeUnknown<ID3D11SamplerState>(nullptr);

    auto device = _context->getDevice();
    auto deviceContext = _context->getDeviceContext();
    auto vertexShaderBuffer = core::disposing::makeUnknown<ID3DBlob>(nullptr);
    auto pixelShaderBuffer = core::disposing::makeUnknown<ID3DBlob>(nullptr);

    ID3DBlob *error = nullptr;
    ID3DBlob *shader = nullptr;

    std::string vsCode = _passSettings->getVertexShaderCode();
    std::string psCode = _passSettings->getPixelShaderCode();

    auto result = ::D3DCompile(
        vsCode.c_str(),
        vsCode.length(),
        NULL,
        NULL,
        NULL,
        _passSettings->getVertexShaderFunctionName(),
        ENCRYPT_STRING("vs_5_0"),
        D3DCOMPILE_ENABLE_STRICTNESS,
        0,
        &shader,
        &error);

    if (FAILED(result) || shader == nullptr) {
        std::string compilationError = ENCRYPT_STRING("[-]");
        if (error != nullptr) {
            compilationError = (char*)error->GetBufferPointer();
        }

        SAFE_RELEASE(error);
        throw core::exception::InitializationException(
            ENCRYPT_STRING("direct3d11::Pass"),
            core::stringFormat(
                ENCRYPT_STRING("vertex shader compilation failed: %s"),
                compilationError.c_str()));
    }

    vertexShaderBuffer.reset(shader);
    shader = nullptr;

    result = ::D3DCompile(
        psCode.c_str(),
        psCode.length(),
        NULL,
        NULL,
        NULL,
        _passSettings->getPixelShaderFunctionName(),
        ENCRYPT_STRING("ps_5_0"),
        D3DCOMPILE_ENABLE_STRICTNESS,
        0,
        &shader,
        &error);

    if (FAILED(result) || shader == nullptr) {
        std::string compilationError = ENCRYPT_STRING("[-]");
        if (error != nullptr) {
            compilationError = (char*)error->GetBufferPointer();
        }

        SAFE_RELEASE(error);
        throw core::exception::InitializationException(
            ENCRYPT_STRING("direct3d11::Pass"),
            core::stringFormat(
                ENCRYPT_STRING("pixel shader compilation failed: %s"),
                compilationError.c_str()));
    }

    pixelShaderBuffer.reset(shader);
    ID3D11VertexShader *vertexShader = nullptr;

    result = device->CreateVertexShader(vertexShaderBuffer->GetBufferPointer(), vertexShaderBuffer->GetBufferSize(), NULL, &vertexShader);
    if (FAILED(result)) {
        throw core::exception::InitializationException(
            ENCRYPT_STRING("direct3d11::Pass"),
            ENCRYPT_STRING("vertex shader creation failed"));
    }

    _vertexShader.reset(vertexShader);

    ID3D11PixelShader *pixelShader = nullptr;
    result = device->CreatePixelShader(pixelShaderBuffer->GetBufferPointer(), pixelShaderBuffer->GetBufferSize(), NULL, &pixelShader);
    if (FAILED(result)) {
        throw core::exception::InitializationException(
            ENCRYPT_STRING("direct3d11::Pass"),
            ENCRYPT_STRING("pixel shader creation failed"));
    }

    _pixelShader.reset(pixelShader);

    D3D11_INPUT_ELEMENT_DESC polygonLayout[2];
    polygonLayout[0].SemanticName = ENCRYPT_STRING("POSITION");
    polygonLayout[0].SemanticIndex = 0;
    polygonLayout[0].Format = DXGI_FORMAT_R32G32B32_FLOAT;
    polygonLayout[0].InputSlot = 0;
    polygonLayout[0].AlignedByteOffset = 0;
    polygonLayout[0].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
    polygonLayout[0].InstanceDataStepRate = 0;

    polygonLayout[1].SemanticName = ENCRYPT_STRING("TEXCOORD");
    polygonLayout[1].SemanticIndex = 0;
    polygonLayout[1].Format = DXGI_FORMAT_R32G32_FLOAT;
    polygonLayout[1].InputSlot = 0;
    polygonLayout[1].AlignedByteOffset = D3D11_APPEND_ALIGNED_ELEMENT;
    polygonLayout[1].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
    polygonLayout[1].InstanceDataStepRate = 0;

    unsigned int count = sizeof(polygonLayout) / sizeof(polygonLayout[0]);
    ID3D11InputLayout *inputLayout = nullptr;
    result = device->CreateInputLayout(polygonLayout, count, vertexShaderBuffer->GetBufferPointer(), vertexShaderBuffer->GetBufferSize(), &inputLayout);
    if (FAILED(result)) {
        throw core::exception::InitializationException(
            ENCRYPT_STRING("direct3d11::Pass"),
            ENCRYPT_STRING("input layout creation failed"));
    }

    _inputLayout.reset(inputLayout);

    D3D11_BUFFER_DESC matrixBufferDescription;
    matrixBufferDescription.Usage = D3D11_USAGE_DYNAMIC;
    matrixBufferDescription.ByteWidth = sizeof(direct3d11::dto::MatrixBufferType);
    matrixBufferDescription.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    matrixBufferDescription.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    matrixBufferDescription.MiscFlags = 0;
    matrixBufferDescription.StructureByteStride = 0;

    ID3D11Buffer *matrixBuffer = nullptr;
    result = device->CreateBuffer(&matrixBufferDescription, NULL, &matrixBuffer);
    if (FAILED(result)) {
        throw core::exception::InitializationException(
            ENCRYPT_STRING("direct3d11::Pass"),
            ENCRYPT_STRING("buffer creation failed"));
    }

    _matrixBuffer.reset(matrixBuffer);

    D3D11_SAMPLER_DESC samplerDescription;
    samplerDescription.MaxAnisotropy = 1;
    samplerDescription.AddressU = D3D11_TEXTURE_ADDRESS_CLAMP;
    samplerDescription.AddressV = D3D11_TEXTURE_ADDRESS_CLAMP;
    samplerDescription.AddressW = D3D11_TEXTURE_ADDRESS_CLAMP;
    samplerDescription.MipLODBias = 0;
    samplerDescription.ComparisonFunc = D3D11_COMPARISON_NEVER;
    samplerDescription.MinLOD = 0;
    samplerDescription.MaxLOD = D3D11_FLOAT32_MAX;

    samplerDescription.Filter = D3D11_FILTER_MIN_MAG_MIP_POINT;
    _pointSampler.reset(::createSampler(device.get(), samplerDescription));

    samplerDescription.Filter = D3D11_FILTER_MIN_MAG_LINEAR_MIP_POINT;
    _bilinearSampler.reset(::createSampler(device.get(), samplerDescription));

    _states.push_back(std::shared_ptr<direct3d11::IState>(new direct3d11::DeviceContextVSConstantBuffers(deviceContext.get())));
    _states.push_back(std::shared_ptr<direct3d11::IState>(new direct3d11::DeviceContextPSShaderResources(deviceContext.get())));
    _states.push_back(std::shared_ptr<direct3d11::IState>(new direct3d11::DeviceContextIAInputLayout(deviceContext.get())));
    _states.push_back(std::shared_ptr<direct3d11::IState>(new direct3d11::DeviceContextVSShader(deviceContext.get())));
    _states.push_back(std::shared_ptr<direct3d11::IState>(new direct3d11::DeviceContextPSShader(deviceContext.get())));
    _states.push_back(std::shared_ptr<direct3d11::IState>(new direct3d11::DeviceContextPSSamplers(deviceContext.get())));
    _states.push_back(std::shared_ptr<direct3d11::IState>(new direct3d11::DeviceContextRSViewports(deviceContext.get())));
}

void direct3d11::Pass::render() {
    direct3d11::utility::setNoRenderer(_context);
    _renderer->begin();

    auto worldMatrix = DirectX::XMMatrixTranspose(DirectX::XMLoadFloat4x4(&_matrixBufferType.world));
    auto viewMatrix = DirectX::XMMatrixTranspose(DirectX::XMLoadFloat4x4(&_matrixBufferType.view));
    auto projectionMatrix = DirectX::XMMatrixTranspose(DirectX::XMLoadFloat4x4(&_matrixBufferType.projection));

    auto deviceContext = _context->getDeviceContext();
    auto matrixBuffer = _matrixBuffer.get();

    D3D11_MAPPED_SUBRESOURCE resource;
    auto result = deviceContext->Map(matrixBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &resource);
    if (FAILED(result)) {
        throw core::exception::OperationException(
            ENCRYPT_STRING("direct3d11::Pass"),
            ENCRYPT_STRING("resource could not be mapped"));
    }

    auto data = (direct3d11::dto::MatrixBufferType*)resource.pData;
    DirectX::XMStoreFloat4x4(&data->world, worldMatrix);
    DirectX::XMStoreFloat4x4(&data->view, viewMatrix);
    DirectX::XMStoreFloat4x4(&data->projection, projectionMatrix);
    deviceContext->Unmap(matrixBuffer, 0);

    for (auto state : _states) {
        state->acquire();
    }

    D3D11_VIEWPORT viewport;
    ZeroMemory(&viewport, sizeof(D3D11_VIEWPORT));
    viewport.Width = (float)_resolution.width;
    viewport.Height = (float)_resolution.height;
    viewport.MinDepth = 0.0f;
    viewport.MaxDepth = 1.0f;
    viewport.TopLeftX = 0.0f;
    viewport.TopLeftY = 0.0f;
    deviceContext->RSSetViewports(1, &viewport);

    core::type_slot inCount;
    auto in = _passSettings->getIn(inCount);
    deviceContext->PSSetShaderResources(0, inCount, in);

    core::type_slot outCount;
    auto out = _passSettings->getOut(outCount);
    deviceContext->OMSetRenderTargets(outCount, out, NULL);

    deviceContext->VSSetConstantBuffers(0, 1, &matrixBuffer);
    deviceContext->IASetInputLayout(_inputLayout.get());
    deviceContext->VSSetShader(_vertexShader.get(), NULL, 0);
    deviceContext->PSSetShader(_pixelShader.get(), NULL, 0);

    auto sampler = _pointSampler.get();
    deviceContext->PSSetSamplers(0, 1, &sampler);

    sampler = _bilinearSampler.get();
    deviceContext->PSSetSamplers(1, 1, &sampler);

    _renderer->render();

    for (auto state : _states) {
        state->restore();
    }

    _renderer->end();
}