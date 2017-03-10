#include "stdafx.h"
#include "gamesettingsrepository.h"

core::dto::GameSettings core::GameSettingsRepository::load() {
    return _settings;
}

void core::GameSettingsRepository::update(core::dto::Direct3D11Settings settings) {
    _settings.direct3D11Settings = settings;
}