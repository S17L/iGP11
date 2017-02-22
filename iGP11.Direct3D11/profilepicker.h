#pragma once

#include "stdafx.h"
#include "igp11core.h"
#include "igp11direct3d11.h"
#include "darksouls2profile.h"
#include "darksouls3profile.h"
#include "fallout4profile.h"
#include "genericprofile.h"

namespace direct3d11 {
	typedef std::function<IProfile*(direct3d11::ProfileConfiguration)> ProfileFactoryFunction;

	class ProfilePicker : public IProfilePicker {
    private:
        std::map<core::Direct3D11ProfileType, ProfileFactoryFunction> _picker;
	public:
        ProfilePicker();
		virtual ~ProfilePicker() {}
		virtual std::unique_ptr<IProfile> getProfile(core::Direct3D11ProfileType profileType, ProfileConfiguration configuration) override;
	};
}