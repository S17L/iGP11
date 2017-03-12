#include "stdafx.h"
#include "effect.h"

direct3d11::EffectsApplicator::EffectsApplicator(dto::FilterSettings filterSettings, direct3d11::Direct3D11Context *context) {
    _filterSettings = filterSettings;
    _requestedFilterSettings = filterSettings;
    _context = context;
}

void direct3d11::EffectsApplicator::addEffect(direct3d11::IEffect *effect) {
    _effects.push_back(std::shared_ptr<direct3d11::IEffect>(effect));
}

void direct3d11::EffectsApplicator::applyProcessing(const direct3d11::dto::PostProcessingSettings &postProcessingSettings) {
    if (postProcessingSettings.colorTexture == nullptr) {
        return;
    }

    if (initializationRequired(postProcessingSettings)) {
        auto resolution = direct3d11::utility::getRenderingResolution(postProcessingSettings.colorTexture);
        _filterSettings = _requestedFilterSettings;
        _resolution = resolution;

        clear();

        if (_resolution.width > 0 && _resolution.height > 0) {
            ThreadLoggerAppenderScope scope(ENCRYPT_STRING("direct3d11::EffectsApplicator"));

            log(core::stringFormat(ENCRYPT_STRING("resolution: [ width: %u, height: %u ]"), _resolution.width, _resolution.height));
            _proxy.reset(new direct3d11::RenderingProxy(_context, _resolution, postProcessingSettings.colorTexture, postProcessingSettings.depthTexture));
            _codeBuilderFactory.reset(new direct3d11::ShaderCodeFactory(_filterSettings.codeDirectoryPath, _resolution));

            if (_filterSettings.pluginSettings.renderingMode == core::RenderingMode::alpha) {
                addEffect(new direct3d11::AlphaEffect(_context, _proxy->nextColorTexture(), _resolution, _codeBuilderFactory.get()));
            }
            else if (_filterSettings.pluginSettings.renderingMode == core::RenderingMode::depthbuffer) {
                if (postProcessingSettings.depthTexture != nullptr) {
                    addEffect(new direct3d11::DepthEffect(_context, _proxy->nextColorTexture(), _proxy->getDepthTextureView(), _resolution, _filterSettings.depthBuffer, _codeBuilderFactory.get()));
                }
            }
            else if (_filterSettings.pluginSettings.renderingMode == core::RenderingMode::luminescence) {
                addEffect(new direct3d11::LuminescenceEffect(_context, _proxy->nextColorTexture(), _resolution, _codeBuilderFactory.get()));
            }
            else if (_filterSettings.pluginSettings.renderingMode == core::RenderingMode::effects) {
                if (_filterSettings.denoise.isEnabled) {
                    addEffect(new direct3d11::DenoiseEffect(_context, _proxy->nextColorTexture(), _resolution, _filterSettings.denoise, _codeBuilderFactory.get()));
                }
                
                if (_filterSettings.tonemap.isEnabled) {
                    addEffect(new direct3d11::TonemapEffect(_context, _proxy->nextColorTexture(), _resolution, _filterSettings.tonemap, _codeBuilderFactory.get()));
                }

                if (_filterSettings.vibrance.isEnabled) {
                    addEffect(new direct3d11::VibranceEffect(_context, _proxy->nextColorTexture(), _resolution, _filterSettings.vibrance, _codeBuilderFactory.get()));
                }

                if (_filterSettings.liftGammaGain.isEnabled) {
                    addEffect(new direct3d11::LiftGammaGainEffect(_context, _proxy->nextColorTexture(), _resolution, _filterSettings.liftGammaGain, _codeBuilderFactory.get()));
                }

                if (_filterSettings.lumaSharpen.isEnabled) {
                    addEffect(new direct3d11::LumasharpenEffect(_context, _proxy->nextColorTexture(), _resolution, _filterSettings.lumaSharpen, _codeBuilderFactory.get()));
                }

                if (_filterSettings.bokehDoF.isEnabled && postProcessingSettings.depthTexture != nullptr) {
                    addEffect(new direct3d11::BokehDoFEffect(_context, _proxy->nextColorTexture(), _proxy->getDepthTextureView(), _resolution, _filterSettings.bokehDoF, _filterSettings.depthBuffer, _codeBuilderFactory.get()));
                }
            }

            _currentColorTexture = postProcessingSettings.colorTexture;
            _currentDepthTexture = postProcessingSettings.depthTexture;
        }

        _initRequested = false;
    }

    auto size = _effects.size();
    if (size <= 0) {
        return;
    }

    ThreadLoggerAppenderScope scope(
        debug,
        core::stringFormat(ENCRYPT_STRING("applying %llu effects"), size));

    _proxy->begin();

    for (auto effect : _effects) {
        log(debug, effect->getName());
        _proxy->iterate();
        direct3d11::utility::setNoRenderer(_context);
        effect->begin();
        _proxy->getRenderingDestinationTexture()->setAsRenderer();
        effect->render();
        effect->end();
    }

    _proxy->end();
}

void direct3d11::EffectsApplicator::clear() {
    _effects.clear();
    _proxy.reset();
    _codeBuilderFactory.reset();
}

void direct3d11::EffectsApplicator::apply(const direct3d11::dto::PostProcessingSettings &postProcessingSettings) {
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

void direct3d11::EffectsApplicator::deinitialize() {
    _hasError = false;
    _initRequested = false;
    _resolution.height = 0;
    _resolution.width = 0;
    _currentColorTexture = nullptr;
    _currentDepthTexture = nullptr;

    clear();
}

bool direct3d11::EffectsApplicator::initializationRequired(const direct3d11::dto::PostProcessingSettings &postProcessingSettings) {
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

void direct3d11::EffectsApplicator::update(dto::FilterSettings filterSettings) {
    _requestedFilterSettings = filterSettings;
    _hasError = false;
    _initRequested = true;
}