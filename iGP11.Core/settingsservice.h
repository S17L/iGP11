#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace core {
	class SettingsService : public ISettingsService {
	private:
		core::ISerializer *_serializer;
	public:
		SettingsService(core::ISerializer *serializer)
			: _serializer(serializer) {}
		virtual core::dto::InjectionSettings getSettings() override;
	};
}