#include "stdafx.h"
#include "injectionsettingsrepository.h"

core::dto::InjectionSettings core::InjectionSettingsRepository::load() {
    return _settings;
}

void core::InjectionSettingsRepository::update(core::dto::Direct3D11Settings settings) {
    _settings.direct3D11Settings = settings;
}