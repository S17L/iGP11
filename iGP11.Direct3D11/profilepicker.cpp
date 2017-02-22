#include "stdafx.h"
#include "profilepicker.h"

direct3d11::ProfilePicker::ProfilePicker() {
    _picker = {
        { core::Direct3D11ProfileType::generic, [](direct3d11::ProfileConfiguration configuration) { return new direct3d11::GenericProfile(configuration); } },
        { core::Direct3D11ProfileType::darksouls2, [](direct3d11::ProfileConfiguration configuration) { return new direct3d11::DarkSouls2Profile(configuration); } },
        { core::Direct3D11ProfileType::darksouls3, [](direct3d11::ProfileConfiguration configuration) { return new direct3d11::DarkSouls3Profile(configuration); } },
        { core::Direct3D11ProfileType::fallout4, [](direct3d11::ProfileConfiguration configuration) { return new direct3d11::Fallout4Profile(configuration); } }
    };
}

std::unique_ptr<direct3d11::IProfile> direct3d11::ProfilePicker::getProfile(core::Direct3D11ProfileType type, direct3d11::ProfileConfiguration configuration) {
	auto iterator = _picker.find(type);
	auto pair = iterator != _picker.end() ? iterator : _picker.find(core::Direct3D11ProfileType::generic);

	return std::unique_ptr<direct3d11::IProfile>(pair->second(configuration));
}