#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace core {
    class GameSettingsRepository : public IGameSettingsRepository {
    private:
        dto::GameSettings _settings;
    public:
        GameSettingsRepository(dto::GameSettings settings)
            : _settings(settings) {}
        ~GameSettingsRepository() {}
        virtual dto::GameSettings load() override;
        virtual void update(dto::Direct3D11Settings settings) override;
    };
}