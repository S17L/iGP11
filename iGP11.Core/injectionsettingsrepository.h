#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace core {
    class InjectionSettingsRepository : public IInjectionSettingsRepository {
    private:
        dto::InjectionSettings _settings;
    public:
        InjectionSettingsRepository(dto::InjectionSettings settings)
            : _settings(settings) {}
        ~InjectionSettingsRepository() {}
        virtual dto::InjectionSettings load() override;
        virtual void update(dto::Direct3D11Settings settings) override;
    };
}