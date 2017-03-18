using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [ComponentName("Textures")]
    [ComponentShortDescription("TexturesShortDescription")]
    [ComponentLongDescription("TexturesLongDescription")]
    [DataContract]
    [NoValidation]
    public class Textures
    {
        [ComponentName("DetailLevel")]
        [DataMember(Name = "detailLevel", EmitDefaultValue = true)]
        [Editable(FormType.New)]
        [Order(0)]
        public TextureDetailLevel DetailLevel { get; set; }

        [ComponentName("DumpingPath")]
        [DataMember(Name = "dumpingPath", EmitDefaultValue = true)]
        [DirectoryPath]
        [Editable]
        [Order(3)]
        [Tokenizable]
        public string DumpingPath { get; set; }

        [ComponentName("OverrideMode")]
        [DataMember(Name = "overrideMode", EmitDefaultValue = true)]
        [Editable(FormType.New)]
        [Order(1)]
        public TextureOverrideMode OverrideMode { get; set; }

        [ComponentName("OverridePath")]
        [DataMember(Name = "overridePath", EmitDefaultValue = true)]
        [DirectoryPath]
        [Editable]
        [Order(2)]
        [Tokenizable]
        public string OverridePath { get; set; }
    }
}