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

void write(rapidjson::Writer<rapidjson::StringBuffer> &writer, const core::dto::Direct3D11Settings &data) {
    writer.StartObject();
    {
        writer.String(ENCRYPT_STRING("bokehDoF"));
        writer.StartObject();
        {
            writer.String(ENCRYPT_STRING("isEnabled"));
            writer.Bool(data.bokehDoF.isEnabled);
            writer.String(ENCRYPT_STRING("isChromaticAberrationEnabled"));
            writer.Bool(data.bokehDoF.isChromaticAberrationEnabled);
            writer.String(ENCRYPT_STRING("chromaticAberrationFringe"));
            writer.Double(data.bokehDoF.chromaticAberrationFringe);
            writer.String(ENCRYPT_STRING("isPreservingShape"));
            writer.Bool(data.bokehDoF.isPreservingShape);
            writer.String(ENCRYPT_STRING("shapeSize"));
            writer.Uint(data.bokehDoF.shapeSize);
            writer.String(ENCRYPT_STRING("shapeStrength"));
            writer.Double(data.bokehDoF.shapeStrength);
            writer.String(ENCRYPT_STRING("shapeRotation"));
            writer.Double(data.bokehDoF.shapeRotation);
            writer.String(ENCRYPT_STRING("isBlurEnabled"));
            writer.Bool(data.bokehDoF.isBlurEnabled);
            writer.String(ENCRYPT_STRING("blurStrength"));
            writer.Double(data.bokehDoF.blurStrength);
            writer.String(ENCRYPT_STRING("depthMinimum"));
            writer.Double(data.bokehDoF.depthMinimum);
            writer.String(ENCRYPT_STRING("depthMaximum"));
            writer.Double(data.bokehDoF.depthMaximum);
            writer.String(ENCRYPT_STRING("depthRateGain"));
            writer.Double(data.bokehDoF.depthRateGain);
            writer.String(ENCRYPT_STRING("luminescenceMinimum"));
            writer.Double(data.bokehDoF.luminescenceMinimum);
            writer.String(ENCRYPT_STRING("luminescenceMaximum"));
            writer.Double(data.bokehDoF.luminescenceMaximum);
            writer.String(ENCRYPT_STRING("luminescenceRateGain"));
            writer.Double(data.bokehDoF.luminescenceRateGain);
            writer.EndObject();
        }

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

        writer.String(ENCRYPT_STRING("pluginSettings"));
        writer.StartObject();
        {
            writer.String(ENCRYPT_STRING("profileType"));
            writer.Int(static_cast<int>(data.pluginSettings.profileType));
            writer.String(ENCRYPT_STRING("renderingMode"));
            writer.Int(static_cast<int>(data.pluginSettings.renderingMode));
            writer.EndObject();
        }

        writer.String(ENCRYPT_STRING("liftGammaGain"));
        writer.StartObject();
        {
            writer.String(ENCRYPT_STRING("isEnabled"));
            writer.Bool(data.liftGammaGain.isEnabled);
            writer.String(ENCRYPT_STRING("liftRed"));
            writer.Double(data.liftGammaGain.lift.red);
            writer.String(ENCRYPT_STRING("liftGreen"));
            writer.Double(data.liftGammaGain.lift.green);
            writer.String(ENCRYPT_STRING("liftBlue"));
            writer.Double(data.liftGammaGain.lift.blue);
            writer.String(ENCRYPT_STRING("gammaRed"));
            writer.Double(data.liftGammaGain.gamma.red);
            writer.String(ENCRYPT_STRING("gammaGreen"));
            writer.Double(data.liftGammaGain.gamma.green);
            writer.String(ENCRYPT_STRING("gammaBlue"));
            writer.Double(data.liftGammaGain.gamma.blue);
            writer.String(ENCRYPT_STRING("gainRed"));
            writer.Double(data.liftGammaGain.gain.red);
            writer.String(ENCRYPT_STRING("gainGreen"));
            writer.Double(data.liftGammaGain.gain.green);
            writer.String(ENCRYPT_STRING("gainBlue"));
            writer.Double(data.liftGammaGain.gain.blue);
            writer.EndObject();
        }

        writer.String(ENCRYPT_STRING("lumaSharpen"));
        writer.StartObject();
        {
            writer.String(ENCRYPT_STRING("isEnabled"));
            writer.Bool(data.lumaSharpen.isEnabled);
            writer.String(ENCRYPT_STRING("sharpeningStrength"));
            writer.Double(data.lumaSharpen.sharpeningStrength);
            writer.String(ENCRYPT_STRING("sharpeningClamp"));
            writer.Double(data.lumaSharpen.sharpeningClamp);
            writer.String(ENCRYPT_STRING("offset"));
            writer.Double(data.lumaSharpen.offset);
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

        writer.String(ENCRYPT_STRING("tonemap"));
        writer.StartObject();
        {
            writer.String(ENCRYPT_STRING("isEnabled"));
            writer.Bool(data.tonemap.isEnabled);
            writer.String(ENCRYPT_STRING("gamma"));
            writer.Double(data.tonemap.gamma);
            writer.String(ENCRYPT_STRING("exposure"));
            writer.Double(data.tonemap.exposure);
            writer.String(ENCRYPT_STRING("saturation"));
            writer.Double(data.tonemap.saturation);
            writer.String(ENCRYPT_STRING("bleach"));
            writer.Double(data.tonemap.bleach);
            writer.String(ENCRYPT_STRING("defog"));
            writer.Double(data.tonemap.defog);
            writer.String(ENCRYPT_STRING("fogRed"));
            writer.Double(data.tonemap.fog.red);
            writer.String(ENCRYPT_STRING("fogGreen"));
            writer.Double(data.tonemap.fog.green);
            writer.String(ENCRYPT_STRING("fogBlue"));
            writer.Double(data.tonemap.fog.blue);
            writer.EndObject();
        }

        writer.String(ENCRYPT_STRING("vibrance"));
        writer.StartObject();
        {
            writer.String(ENCRYPT_STRING("isEnabled"));
            writer.Bool(data.vibrance.isEnabled);
            writer.String(ENCRYPT_STRING("strength"));
            writer.Double(data.vibrance.strength);
            writer.String(ENCRYPT_STRING("gainRed"));
            writer.Double(data.vibrance.gain.red);
            writer.String(ENCRYPT_STRING("gainGreen"));
            writer.Double(data.vibrance.gain.green);
            writer.String(ENCRYPT_STRING("gainBlue"));
            writer.Double(data.vibrance.gain.blue);
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

core::dto::GameSettings core::JsonSerializer::deserializeSettings(const std::string &value) {
    rapidjson::Document document;
    document.Parse(value.c_str());

    core::dto::GameSettings gameSettings;
    rapidjson::Value &gameProfileJson = document[ENCRYPT_STRING("gameProfile")];
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
    rapidjson::Value &bokehDoFJson = document[ENCRYPT_STRING("bokehDoF")];
    if (!bokehDoFJson.IsNull()) {
        core::dto::BokehDoF bokehDoF;
        bokehDoF.isEnabled = bokehDoFJson[ENCRYPT_STRING("isEnabled")].GetBool();
        bokehDoF.isChromaticAberrationEnabled = bokehDoFJson[ENCRYPT_STRING("isChromaticAberrationEnabled")].GetBool();
        bokehDoF.chromaticAberrationFringe = bokehDoFJson[ENCRYPT_STRING("chromaticAberrationFringe")].GetDouble();
        bokehDoF.isPreservingShape = bokehDoFJson[ENCRYPT_STRING("isPreservingShape")].GetBool();
        bokehDoF.shapeSize = bokehDoFJson[ENCRYPT_STRING("shapeSize")].GetUint();
        bokehDoF.shapeStrength = bokehDoFJson[ENCRYPT_STRING("shapeStrength")].GetDouble();
        bokehDoF.shapeRotation = bokehDoFJson[ENCRYPT_STRING("shapeRotation")].GetDouble();
        bokehDoF.isBlurEnabled = bokehDoFJson[ENCRYPT_STRING("isBlurEnabled")].GetBool();
        bokehDoF.blurStrength = bokehDoFJson[ENCRYPT_STRING("blurStrength")].GetDouble();
        bokehDoF.depthMinimum = bokehDoFJson[ENCRYPT_STRING("depthMinimum")].GetDouble();
        bokehDoF.depthMaximum = bokehDoFJson[ENCRYPT_STRING("depthMaximum")].GetDouble();
        bokehDoF.depthRateGain = bokehDoFJson[ENCRYPT_STRING("depthRateGain")].GetDouble();
        bokehDoF.luminescenceMinimum = bokehDoFJson[ENCRYPT_STRING("luminescenceMinimum")].GetDouble();
        bokehDoF.luminescenceMaximum = bokehDoFJson[ENCRYPT_STRING("luminescenceMaximum")].GetDouble();
        bokehDoF.luminescenceRateGain = bokehDoFJson[ENCRYPT_STRING("luminescenceRateGain")].GetDouble();
        settings.bokehDoF = bokehDoF;
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

    rapidjson::Value &liftGammaGainJson = document[ENCRYPT_STRING("liftGammaGain")];
    if (!liftGammaGainJson.IsNull()) {
        core::dto::LiftGammaGain liftGammaGain;
        liftGammaGain.isEnabled = liftGammaGainJson[ENCRYPT_STRING("isEnabled")].GetBool();
        liftGammaGain.lift.red = liftGammaGainJson[ENCRYPT_STRING("liftRed")].GetDouble();
        liftGammaGain.lift.green = liftGammaGainJson[ENCRYPT_STRING("liftGreen")].GetDouble();
        liftGammaGain.lift.blue = liftGammaGainJson[ENCRYPT_STRING("liftBlue")].GetDouble();
        liftGammaGain.gamma.red = liftGammaGainJson[ENCRYPT_STRING("gammaRed")].GetDouble();
        liftGammaGain.gamma.green = liftGammaGainJson[ENCRYPT_STRING("gammaGreen")].GetDouble();
        liftGammaGain.gamma.blue = liftGammaGainJson[ENCRYPT_STRING("gammaBlue")].GetDouble();
        liftGammaGain.gain.red = liftGammaGainJson[ENCRYPT_STRING("gainRed")].GetDouble();
        liftGammaGain.gain.green = liftGammaGainJson[ENCRYPT_STRING("gainGreen")].GetDouble();
        liftGammaGain.gain.blue = liftGammaGainJson[ENCRYPT_STRING("gainBlue")].GetDouble();
        settings.liftGammaGain = liftGammaGain;
    }

    rapidjson::Value &lumaSharpenJson = document[ENCRYPT_STRING("lumaSharpen")];
    if (!lumaSharpenJson.IsNull()) {
        core::dto::LumaSharpen lumaSharpen;
        lumaSharpen.isEnabled = lumaSharpenJson[ENCRYPT_STRING("isEnabled")].GetBool();
        lumaSharpen.sharpeningStrength = lumaSharpenJson[ENCRYPT_STRING("sharpeningStrength")].GetDouble();
        lumaSharpen.sharpeningClamp = lumaSharpenJson[ENCRYPT_STRING("sharpeningClamp")].GetDouble();
        lumaSharpen.offset = lumaSharpenJson[ENCRYPT_STRING("offset")].GetDouble();
        settings.lumaSharpen = lumaSharpen;
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

    rapidjson::Value &tonemapJson = document[ENCRYPT_STRING("tonemap")];
    if (!tonemapJson.IsNull()) {
        core::dto::Tonemap tonemap;
        tonemap.isEnabled = tonemapJson[ENCRYPT_STRING("isEnabled")].GetBool();
        tonemap.gamma = tonemapJson[ENCRYPT_STRING("gamma")].GetDouble();
        tonemap.exposure = tonemapJson[ENCRYPT_STRING("exposure")].GetDouble();
        tonemap.saturation = tonemapJson[ENCRYPT_STRING("saturation")].GetDouble();
        tonemap.bleach = tonemapJson[ENCRYPT_STRING("bleach")].GetDouble();
        tonemap.defog = tonemapJson[ENCRYPT_STRING("defog")].GetDouble();
        tonemap.fog.red = tonemapJson[ENCRYPT_STRING("fogRed")].GetDouble();
        tonemap.fog.green = tonemapJson[ENCRYPT_STRING("fogGreen")].GetDouble();
        tonemap.fog.blue = tonemapJson[ENCRYPT_STRING("fogBlue")].GetDouble();
        settings.tonemap = tonemap;
    }

    rapidjson::Value &vibranceJson = document[ENCRYPT_STRING("vibrance")];
    if (!vibranceJson.IsNull()) {
        core::dto::Vibrance vibrance;
        vibrance.isEnabled = vibranceJson[ENCRYPT_STRING("isEnabled")].GetBool();
        vibrance.strength = vibranceJson[ENCRYPT_STRING("strength")].GetDouble();
        vibrance.gain.red = vibranceJson[ENCRYPT_STRING("gainRed")].GetDouble();
        vibrance.gain.green = vibranceJson[ENCRYPT_STRING("gainGreen")].GetDouble();
        vibrance.gain.blue = vibranceJson[ENCRYPT_STRING("gainBlue")].GetDouble();
        settings.vibrance = vibrance;
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