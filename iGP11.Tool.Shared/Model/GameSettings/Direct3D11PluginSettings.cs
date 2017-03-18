using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [ComponentName("PluginSettings")]
    [ComponentShortDescription("PluginSettingsShortDescription")]
    [ComponentLongDescription("PluginSettingsLongDescription")]
    [DataContract]
    public class Direct3D11PluginSettings
    {
        [ComponentName("ProfileType")]
        [DataMember(Name = "profileType")]
        [Editable(FormType.New)]
        public Direct3D11ProfileType ProfileType { get; set; }

        [ComponentName("RenderingMode")]
        [DataMember(Name = "renderingMode")]
        [Editable]
        public RenderingMode RenderingMode { get; set; }
    }
}