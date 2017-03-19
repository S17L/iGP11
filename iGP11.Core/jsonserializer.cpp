#include "stdafx.h"
#include "jsonserializer.h"

#include "rapidjson\document.h"
#include "rapidjson\writer.h"
#include "rapidjson\stringbuffer.h"

std::string toString(rapidjson::Value &value) {
    rapidjson::StringBuffer stringBuffer;
    rapidjson::Writer<rapidjson::StringBuffer> writer(stringBuffer);
    value.Accept(writer);

    return stringBuffer.GetString();
}

void write(rapidjson::Writer<rapidjson::StringBuffer> &writer, const core::dto::EffectData &data) {
    writer.StartObject();
    {
        writer.String(ENCRYPT_STRING("data"));
        writer.String(data.data.c_str());
        writer.String(ENCRYPT_STRING("id"));
        writer.String(data.id.c_str());
        writer.String(ENCRYPT_STRING("isEnabled"));
        writer.Bool(data.isEnabled);
        writer.String(ENCRYPT_STRING("type"));
        writer.Int(static_cast<int>(data.type));
        writer.EndObject();
    }
}

void write(rapidjson::Writer<rapidjson::StringBuffer> &writer, const core::dto::Direct3D11Settings &data) {
    writer.StartObject();
    {
        writer.String(ENCRYPT_STRING("depthBuffer"));
        writer.StartObject();
        {
            writer.String(ENCRYPT_STRING("linearZNear"));
            writer.Double(data.depthBuffer.linearZNear);
            writer.String(ENCRYPT_STRING("linearZFar"));
            writer.Double(data.depthBuffer.linearZFar);
            writer.String(ENCRYPT_STRING("isLimitEnabled"));
            writer.Bool(data.depthBuffer.isLimitEnabled);
            writer.String(ENCRYPT_STRING("depthMinimum"));
            writer.Double(data.depthBuffer.depthMinimum);
            writer.String(ENCRYPT_STRING("depthMaximum"));
            writer.Double(data.depthBuffer.depthMaximum);
            writer.EndObject();
        }

        writer.String(ENCRYPT_STRING("effects"));
        writer.StartArray();
        {
            for (auto effectData : data.effects) {
                ::write(writer, effectData);
            }

            writer.EndArray();
        }

        writer.String(ENCRYPT_STRING("pluginSettings"));
        writer.StartObject();
        {
            writer.String(ENCRYPT_STRING("profileType"));
            writer.Int(static_cast<int>(data.pluginSettings.profileType));
            writer.String(ENCRYPT_STRING("renderingMode"));
            writer.Int(static_cast<int>(data.pluginSettings.renderingMode));
            writer.EndObject();
        }

        writer.String(ENCRYPT_STRING("textures"));
        writer.StartObject();
        {
            writer.String(ENCRYPT_STRING("detailLevel"));
            writer.Int(static_cast<int>(data.textures.detailLevel));
            writer.String(ENCRYPT_STRING("overrideMode"));
            writer.Int(static_cast<int>(data.textures.overrideMode));
            writer.String(ENCRYPT_STRING("dumpingPath"));
            writer.String(data.textures.dumpingPath.c_str());
            writer.String(ENCRYPT_STRING("overridePath"));
            writer.String(data.textures.overridePath.c_str());
            writer.EndObject();
        }

        writer.EndObject();
    }
}

core::dto::Command core::JsonSerializer::deserializeCommand(const std::string &value) {
    rapidjson::Document document;
    document.Parse(value.c_str());

    core::dto::Command command;
    command.id = document[ENCRYPT_STRING("id")].GetInt();
    command.data = document[ENCRYPT_STRING("data")].GetString();

    return command;
}

core::dto::BokehDoF core::JsonSerializer::deserializeBokehDoF(const std::string &value) {
    rapidjson::Document document;
    document.Parse(value.c_str());

    core::dto::BokehDoF bokehDoF;
    bokehDoF.isChromaticAberrationEnabled = document[ENCRYPT_STRING("isChromaticAberrationEnabled")].GetBool();
    bokehDoF.chromaticAberrationFringe = document[ENCRYPT_STRING("chromaticAberrationFringe")].GetDouble();
    bokehDoF.isPreservingShape = document[ENCRYPT_STRING("isPreservingShape")].GetBool();
    bokehDoF.shapeSize = document[ENCRYPT_STRING("shapeSize")].GetUint();
    bokehDoF.shapeStrength = document[ENCRYPT_STRING("shapeStrength")].GetDouble();
    bokehDoF.shapeRotation = document[ENCRYPT_STRING("shapeRotation")].GetDouble();
    bokehDoF.isBlurEnabled = document[ENCRYPT_STRING("isBlurEnabled")].GetBool();
    bokehDoF.blurStrength = document[ENCRYPT_STRING("blurStrength")].GetDouble();
    bokehDoF.depthMinimum = document[ENCRYPT_STRING("depthMinimum")].GetDouble();
    bokehDoF.depthMaximum = document[ENCRYPT_STRING("depthMaximum")].GetDouble();
    bokehDoF.depthRateGain = document[ENCRYPT_STRING("depthRateGain")].GetDouble();
    bokehDoF.luminescenceMinimum = document[ENCRYPT_STRING("luminescenceMinimum")].GetDouble();
    bokehDoF.luminescenceMaximum = document[ENCRYPT_STRING("luminescenceMaximum")].GetDouble();
    bokehDoF.luminescenceRateGain = document[ENCRYPT_STRING("luminescenceRateGain")].GetDouble();

    return bokehDoF;
}

core::dto::Denoise core::JsonSerializer::deserializeDenoise(const std::string &value) {
    rapidjson::Document document;
    document.Parse(value.c_str());

    core::dto::Denoise denoise;
    denoise.noiseLevel = document[ENCRYPT_STRING("noiseLevel")].GetDouble();
    denoise.blendingCoefficient = document[ENCRYPT_STRING("blendingCoefficient")].GetDouble();
    denoise.weightThreshold = document[ENCRYPT_STRING("weightThreshold")].GetDouble();
    denoise.counterThreshold = document[ENCRYPT_STRING("counterThreshold")].GetDouble();
    denoise.gaussianSigma = document[ENCRYPT_STRING("gaussianSigma")].GetDouble();
    denoise.windowSize = document[ENCRYPT_STRING("windowSize")].GetUint();

    return denoise;
}

core::dto::HDR core::JsonSerializer::deserializeHDR(const std::string &value) {
    rapidjson::Document document;
    document.Parse(value.c_str());

    core::dto::HDR hdr;
    hdr.strength = document[ENCRYPT_STRING("strength")].GetDouble();
    hdr.radius = document[ENCRYPT_STRING("radius")].GetDouble();

    return hdr;
}

core::dto::LiftGammaGain core::JsonSerializer::deserializeLiftGammaGain(const std::string &value) {
    rapidjson::Document document;
    document.Parse(value.c_str());

    core::dto::LiftGammaGain liftGammaGain;
    liftGammaGain.lift.red = document[ENCRYPT_STRING("liftRed")].GetDouble();
    liftGammaGain.lift.green = document[ENCRYPT_STRING("liftGreen")].GetDouble();
    liftGammaGain.lift.blue = document[ENCRYPT_STRING("liftBlue")].GetDouble();
    liftGammaGain.gamma.red = document[ENCRYPT_STRING("gammaRed")].GetDouble();
    liftGammaGain.gamma.green = document[ENCRYPT_STRING("gammaGreen")].GetDouble();
    liftGammaGain.gamma.blue = document[ENCRYPT_STRING("gammaBlue")].GetDouble();
    liftGammaGain.gain.red = document[ENCRYPT_STRING("gainRed")].GetDouble();
    liftGammaGain.gain.green = document[ENCRYPT_STRING("gainGreen")].GetDouble();
    liftGammaGain.gain.blue = document[ENCRYPT_STRING("gainBlue")].GetDouble();

    return liftGammaGain;
}

core::dto::LumaSharpen core::JsonSerializer::deserializeLumaSharpen(const std::string &value) {
    rapidjson::Document document;
    document.Parse(value.c_str());

    core::dto::LumaSharpen lumaSharpen;
    lumaSharpen.sharpeningStrength = document[ENCRYPT_STRING("sharpeningStrength")].GetDouble();
    lumaSharpen.sharpeningClamp = document[ENCRYPT_STRING("sharpeningClamp")].GetDouble();
    lumaSharpen.offset = document[ENCRYPT_STRING("offset")].GetDouble();

    return lumaSharpen;
}

core::dto::Tonemap core::JsonSerializer::deserializeTonemap(const std::string &value) {
    rapidjson::Document document;
    document.Parse(value.c_str());

    core::dto::Tonemap tonemap;
    tonemap.gamma = document[ENCRYPT_STRING("gamma")].GetDouble();
    tonemap.exposure = document[ENCRYPT_STRING("exposure")].GetDouble();
    tonemap.saturation = document[ENCRYPT_STRING("saturation")].GetDouble();
    tonemap.bleach = document[ENCRYPT_STRING("bleach")].GetDouble();
    tonemap.defog = document[ENCRYPT_STRING("defog")].GetDouble();
    tonemap.fog.red = document[ENCRYPT_STRING("fogRed")].GetDouble();
    tonemap.fog.green = document[ENCRYPT_STRING("fogGreen")].GetDouble();
    tonemap.fog.blue = document[ENCRYPT_STRING("fogBlue")].GetDouble();

    return tonemap;
}

core::dto::Vibrance core::JsonSerializer::deserializeVibrance(const std::string &value) {
    rapidjson::Document document;
    document.Parse(value.c_str());

    core::dto::Vibrance vibrance;
    vibrance.strength = document[ENCRYPT_STRING("strength")].GetDouble();
    vibrance.gain.red = document[ENCRYPT_STRING("gainRed")].GetDouble();
    vibrance.gain.green = document[ENCRYPT_STRING("gainGreen")].GetDouble();
    vibrance.gain.blue = document[ENCRYPT_STRING("gainBlue")].GetDouble();

    return vibrance;
}

core::dto::GameSettings core::JsonSerializer::deserializeSettings(const std::string &value) {
    rapidjson::Document document;
    document.Parse(value.c_str());

    core::dto::GameSettings gameSettings;
    gameSettings.gameName = document[ENCRYPT_STRING("gameName")].GetString();

    rapidjson::Value &gameProfileJson = document[ENCRYPT_STRING("gameProfile")];
    gameSettings.gameProfileName = gameProfileJson[ENCRYPT_STRING("name")].GetString();
    gameSettings.pluginType = static_cast<core::PluginType>(gameProfileJson[ENCRYPT_STRING("pluginType")].GetInt());
    gameSettings.communicationAddress = document[ENCRYPT_STRING("communicationAddress")].GetString();
    gameSettings.communicationPort = (unsigned short)document[ENCRYPT_STRING("communicationPort")].GetUint();
    gameSettings.direct3D11PluginPath = document[ENCRYPT_STRING("direct3D11PluginPath")].GetString();

    core::dto::PluginSettings pluginSettings;
    pluginSettings.gameFilePath = document[ENCRYPT_STRING("gameFilePath")].GetString();
    pluginSettings.logsDirectoryPath = gameProfileJson[ENCRYPT_STRING("logsDirectoryPath")].GetString();
    pluginSettings.proxyDirectoryPath = gameProfileJson[ENCRYPT_STRING("proxyDirectoryPath")].GetString();
    gameSettings.pluginSettings = pluginSettings;

    rapidjson::Value &direct3D11Settings = gameProfileJson[ENCRYPT_STRING("direct3D11Settings")];
    if (!direct3D11Settings.IsNull()) {
        gameSettings.direct3D11Settings = deserializeDirect3D11Settings(::toString(direct3D11Settings));
    }

    return gameSettings;
}

core::dto::Direct3D11Settings core::JsonSerializer::deserializeDirect3D11Settings(const std::string &value) {
    rapidjson::Document document;
    document.Parse(value.c_str());

    core::dto::Direct3D11Settings settings;
    rapidjson::Value &effectsJson = document[ENCRYPT_STRING("effects")];
    if (!effectsJson.IsNull() && effectsJson.IsArray()) {
        for (rapidjson::SizeType i = 0; i < effectsJson.Size(); i++) {
            rapidjson::Value &effectDataJson = effectsJson[i];
            core::dto::EffectData effectData;
            effectData.data = effectDataJson[ENCRYPT_STRING("data")].GetString();
            effectData.id = effectDataJson[ENCRYPT_STRING("id")].GetString();
            effectData.isEnabled = effectDataJson[ENCRYPT_STRING("isEnabled")].GetBool();
            effectData.type = static_cast<core::EffectType>(effectDataJson[ENCRYPT_STRING("type")].GetInt());
            settings.effects.push_back(effectData);
        }
    }

    rapidjson::Value &depthBufferJson = document[ENCRYPT_STRING("depthBuffer")];
    if (!depthBufferJson.IsNull()) {
        core::dto::DepthBuffer depthBuffer;
        depthBuffer.linearZNear = depthBufferJson[ENCRYPT_STRING("linearZNear")].GetDouble();
        depthBuffer.linearZFar = depthBufferJson[ENCRYPT_STRING("linearZFar")].GetDouble();
        depthBuffer.isLimitEnabled = depthBufferJson[ENCRYPT_STRING("isLimitEnabled")].GetBool();
        depthBuffer.depthMinimum = depthBufferJson[ENCRYPT_STRING("depthMinimum")].GetDouble();
        depthBuffer.depthMaximum = depthBufferJson[ENCRYPT_STRING("depthMaximum")].GetDouble();
        settings.depthBuffer = depthBuffer;
    }

    rapidjson::Value &pluginSettingsJson = document[ENCRYPT_STRING("pluginSettings")];
    if (!pluginSettingsJson.IsNull()) {
        core::dto::Direct3D11PluginSettings pluginSettings;
        pluginSettings.profileType = static_cast<core::Direct3D11ProfileType>(pluginSettingsJson[ENCRYPT_STRING("profileType")].GetInt());
        pluginSettings.renderingMode = static_cast<core::RenderingMode>(pluginSettingsJson[ENCRYPT_STRING("renderingMode")].GetInt());
        settings.pluginSettings = pluginSettings;
    }

    rapidjson::Value &texturesJson = document[ENCRYPT_STRING("textures")];
    if (!texturesJson.IsNull()) {
        core::dto::Textures textures;
        textures.detailLevel = static_cast<core::TextureDetailLevel>(texturesJson[ENCRYPT_STRING("detailLevel")].GetInt());
        textures.overrideMode = static_cast<core::TextureOverrideMode>(texturesJson[ENCRYPT_STRING("overrideMode")].GetInt());
        textures.dumpingPath = texturesJson[ENCRYPT_STRING("dumpingPath")].GetString();
        textures.overridePath = texturesJson[ENCRYPT_STRING("overridePath")].GetString();
        settings.textures = textures;
    }

    return settings;
}

core::dto::UpdateProxySettings core::JsonSerializer::deserializeUpdateProxySettings(const std::string &value) {
    rapidjson::Document document;
    document.Parse(value.c_str());

    core::dto::UpdateProxySettings settings;
    settings.pluginType = static_cast<core::PluginType>(document[ENCRYPT_STRING("pluginType")].GetInt());

    rapidjson::Value &direct3D11Settings = document[ENCRYPT_STRING("direct3D11Settings")];
    if (!direct3D11Settings.IsNull()) {
        settings.direct3D11Settings = deserializeDirect3D11Settings(::toString(direct3D11Settings));
    }

    return settings;
}

std::string core::JsonSerializer::serialize(core::dto::ProxySettings data) {
    rapidjson::StringBuffer stringBuffer;
    rapidjson::Writer<rapidjson::StringBuffer> writer(stringBuffer);

    writer.StartObject();
    {
        writer.String(ENCRYPT_STRING("gameName"));
        writer.String(data.gameName.c_str());
        writer.String(ENCRYPT_STRING("gameProfileName"));
        writer.String(data.gameProfileName.c_str());
        writer.String(ENCRYPT_STRING("gameFilePath"));
        writer.String(data.gameFilePath.c_str());
        writer.String(ENCRYPT_STRING("proxyDirectoryPath"));
        writer.String(data.proxyDirectoryPath.c_str());
        writer.String(ENCRYPT_STRING("logsDirectoryPath"));
        writer.String(data.logsDirectoryPath.c_str());
        writer.String(ENCRYPT_STRING("pluginType"));
        writer.Int(static_cast<int>(data.pluginType));
        writer.String(ENCRYPT_STRING("activationStatus"));
        writer.Int(static_cast<int>(data.activationStatus));
        writer.String(ENCRYPT_STRING("direct3D11Settings"));
        ::write(writer, data.direct3D11Settings);
    }
    writer.EndObject();

    return stringBuffer.GetString();
}

std::string core::JsonSerializer::serialize(core::dto::Direct3D11Settings data) {
    rapidjson::StringBuffer stringBuffer;
    rapidjson::Writer<rapidjson::StringBuffer> writer(stringBuffer);

    ::write(writer, data);

    return stringBuffer.GetString();
}