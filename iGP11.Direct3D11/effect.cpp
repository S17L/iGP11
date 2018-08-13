#include "stdafx.h"
#include "effect.h"

direct3d11::Technique::Technique(
    Direct3D11Context *context,
    core::Technique technique,
    direct3d11::ITexture *color,
    ID3D11ShaderResourceView *depth,
    direct3d11::ITexture *output) {
    _context = context;
    _technique = technique;
    _color = color;
    _depth = depth;
    _output = output;

    log(
        debug,
        core::stringFormat(
            ENCRYPT_STRING("technique: [ name: %s, color: %s, depth: %p, output: %s]"),
            technique.name.c_str(),
            direct3d11::stringify::toString(color).c_str(),
            depth,
            direct3d11::stringify::toString(output).c_str()));

    log(debug, core::stringFormat("\n%s", technique.code.psCode.c_str()));
    init();
}

void direct3d11::Technique::init() {
    ThreadLoggerAppenderScope scope(core::stringFormat(ENCRYPT_STRING("%s: initialization"), _technique.name.c_str()));
    auto textureDescription = direct3d11::utility::getDescription(_color->get());
    for (auto pair : _technique.textures) {
        ThreadLoggerAppenderScope scope(debug, ENCRYPT_STRING("custom texture"));
        auto texture = std::shared_ptr<direct3d11::ITexture>(
            new direct3d11::Texture(
                _context,
                direct3d11::utility::apply(textureDescription, pair.second.resolution)));

        _texturesById[pair.second.id] = texture;
        _slotById[pair.second.id] = pair.first;

        log(
            debug,
            core::stringFormat(
                ENCRYPT_STRING("id: %u, slot: %u, texture: %s, description: %s"),
                pair.second.id,
                pair.first,
                direct3d11::stringify::toString(texture.get()).c_str(),
                direct3d11::stringify::toString(&textureDescription).c_str()));
    }

    for (auto pass : _technique.passes) {
        ThreadLoggerAppenderScope scope(
            debug,
            core::stringFormat(
                ENCRYPT_STRING("pass: [ vs: %s, ps: %s ]"),
                pass.vsFunctionName.c_str(),
                pass.psFunctionName.c_str()));

        auto passSettings = std::shared_ptr<direct3d11::PassSettings>(
            new direct3d11::PassSettings(
                _technique.code,
                pass.vsFunctionName,
                pass.psFunctionName));

        passSettings->setIn(_technique.color, _color->getInView());
        passSettings->setIn(_technique.depth, _depth);

        for (auto texId : pass.in) {
            passSettings->setIn(_slotById[texId], _texturesById[texId]->getInView());
        }

        core::dto::Resolution resolution;
        core::type_slot outSlot = 0;
        for (auto texId : pass.out) {
            if (texId == core::OutputTexId) {
                passSettings->setOut(outSlot++, _output->getOutView());
                resolution = direct3d11::utility::getRenderingResolution(_output->get());
            }
            else {
                auto texture = _texturesById[texId];
                passSettings->setOut(outSlot++, texture->getOutView());
                resolution = direct3d11::utility::getRenderingResolution(texture->get());
            }
        }

        _passSettings.push_back(passSettings);
        _passes.push_back(std::shared_ptr<direct3d11::Pass>(
            new direct3d11::Pass(
                _context,
                passSettings.get(),
                resolution)));
    }
}

void direct3d11::Technique::render() {
    for (auto pass : _passes) {
        pass->render();
    }
}

direct3d11::TechniqueApplicator::TechniqueApplicator(
    dto::FilterSettings filterSettings,
    direct3d11::Direct3D11Context *context,
    core::ISerializer *serializer) {
    _filterSettings = filterSettings;
    _requestedFilterSettings = filterSettings;
    _context = context;
    _serializer = serializer;
}

void direct3d11::TechniqueApplicator::addTechnique(direct3d11::ITechnique *technique) {
    _techniques.push_back(std::shared_ptr<direct3d11::ITechnique>(technique));
}

void direct3d11::TechniqueApplicator::applyProcessing(const direct3d11::dto::PostProcessingSettings &postProcessingSettings) {
    if (postProcessingSettings.colorTexture == nullptr) {
        return;
    }

    if (initializationRequired(postProcessingSettings)) {
        auto resolution = direct3d11::utility::getRenderingResolution(postProcessingSettings.colorTexture);
        _filterSettings = _requestedFilterSettings;
        _resolution = resolution;

        clear();

        if (_resolution.width > 0 && _resolution.height > 0) {
            ThreadLoggerAppenderScope scope(ENCRYPT_STRING("direct3d11::TechniqueApplicator"));
            log(core::stringFormat(
                ENCRYPT_STRING(
                    "resolution: [ width: %u, height: %u ]"),
                    _resolution.width,
                    _resolution.height));

            _proxy.reset(new direct3d11::RenderingProxy(
                _context,
                postProcessingSettings.colorTexture,
                postProcessingSettings.depthTexture));

#if NDEBUG
            core::HlslTechniqueBuilder techniqueBuilder(_resolution);
#else
            core::HlslTechniqueBuilder techniqueBuilder(_resolution, _filterSettings.codeDirectoryPath);
#endif

            if (_filterSettings.pluginSettings.renderingMode == core::RenderingMode::alpha) {
                addTechnique(
                    new direct3d11::Technique(
                        _context,
                        techniqueBuilder.buildAlpha(),
                        _proxy->iterateIn(),
                        _proxy->getDepthTextureView(),
                        _proxy->iterateOut(true)));
            }
            else if (_filterSettings.pluginSettings.renderingMode == core::RenderingMode::depthbuffer) {
                if (postProcessingSettings.depthTexture != nullptr) {
                    addTechnique(
                        new direct3d11::Technique(
                            _context,
                            techniqueBuilder.buildDepth(_filterSettings.depthBuffer),
                            _proxy->iterateIn(),
                            _proxy->getDepthTextureView(),
                            _proxy->iterateOut(true)));
                }
            }
            else if (_filterSettings.pluginSettings.renderingMode == core::RenderingMode::luminescence) {
                addTechnique(
                    new direct3d11::Technique(
                        _context,
                        techniqueBuilder.buildLuminescence(),
                        _proxy->iterateIn(),
                        _proxy->getDepthTextureView(),
                        _proxy->iterateOut(true)));
            }
            else if (_filterSettings.pluginSettings.renderingMode == core::RenderingMode::effects) {
                auto count = 0;
                auto techniques = core::linq::makeEnumerable(_filterSettings.techniques)
                    .where([&](const core::dto::TechniqueData &data)->bool { return data.isEnabled; })
                    .toList();

                for (auto techniqueData : techniques) {
                    ThreadLoggerAppenderScope scope(
                        core::stringFormat(
                            ENCRYPT_STRING("attempting to add technique: %d"),
                            static_cast<int>(techniqueData.type)));

                    core::HlslTechniqueFactory techniqueFactory(
                        &techniqueBuilder,
                        _serializer,
                        _filterSettings.depthBuffer);

                    count++;
                    addTechnique(
                        new direct3d11::Technique(
                            _context,
                            techniqueFactory.create(techniqueData.type, techniqueData.data),
                            _proxy->iterateIn(),
                            _proxy->getDepthTextureView(),
                            _proxy->iterateOut(count == techniques.size())));
                }
            }

            _currentColorTexture = postProcessingSettings.colorTexture;
            _currentDepthTexture = postProcessingSettings.depthTexture;
        }

        _initRequested = false;
    }

    auto size = _techniques.size();
    if (size <= 0) {
        return;
    }

    ThreadLoggerAppenderScope scope(
        debug,
        core::stringFormat(ENCRYPT_STRING("applying %llu techniques"), size));

    _proxy->begin();

    for (auto technique : _techniques) {
        technique->render();
    }

    _proxy->end();
}

void direct3d11::TechniqueApplicator::clear() {
    _techniques.clear();
    _proxy.reset();
}

void direct3d11::TechniqueApplicator::apply(const direct3d11::dto::PostProcessingSettings &postProcessingSettings) {
    if (_hasError) { 
        return;
    }

    try {
        applyProcessing(postProcessingSettings);
    }
    catch (core::exception::InitializationException const &exception) {
        _hasError = true;
        log(error, core::stringFormat(ENCRYPT_STRING("initialization exception occured: %s"), exception.what()));
    }
    catch (core::exception::OperationException const &exception) {
        _hasError = true;
        log(error, core::stringFormat(ENCRYPT_STRING("operation exception occured: %s"), exception.what()));
    }
    catch (core::exception::ResourceNotFoundException const &exception) {
        _hasError = true;
        log(error, core::stringFormat(ENCRYPT_STRING("resource not found exception occured: %s"), exception.what()));
    }
    catch (std::exception const &exception) {
        _hasError = true;
        log(error, core::stringFormat(ENCRYPT_STRING("unknown exception occured: %s"), exception.what()));
    }
    catch (...) {
        _hasError = true;
        log(error, ENCRYPT_STRING("unknown exception occured"));
    }
}

void direct3d11::TechniqueApplicator::deinitialize() {
    _hasError = false;
    _initRequested = false;
    _resolution.height = 0;
    _resolution.width = 0;
    _currentColorTexture = nullptr;
    _currentDepthTexture = nullptr;

    clear();
}

bool direct3d11::TechniqueApplicator::initializationRequired(const direct3d11::dto::PostProcessingSettings &postProcessingSettings) {
    if (postProcessingSettings.colorTexture == nullptr) {
        return false;
    }

    auto resolution = direct3d11::utility::getRenderingResolution(postProcessingSettings.colorTexture);
    return _resolution.width != resolution.width
        || _resolution.height != resolution.height
        || _currentColorTexture != postProcessingSettings.colorTexture
        || _currentDepthTexture != postProcessingSettings.depthTexture
        || _initRequested;
}

void direct3d11::TechniqueApplicator::update(dto::FilterSettings filterSettings) {
    _requestedFilterSettings = filterSettings;
    _hasError = false;
    _initRequested = true;
}