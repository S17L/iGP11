using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Domain.Model.InjectionSettings
{
    [DataContract]
    public class Textures
    {
        [DataMember(Name = "detailLevel", EmitDefaultValue = true)]
        [Editable(FormType.New)]
        public TextureDetailLevel DetailLevel { get; set; }

        [DataMember(Name = "dumpingPath", EmitDefaultValue = true)]
        [DirectoryPath]
        [Editable]
        [Tokenizable]
        public string DumpingPath { get; set; }

        [DataMember(Name = "overrideMode", EmitDefaultValue = true)]
        [Editable(FormType.New)]
        public TextureOverrideMode OverrideMode { get; set; }

        [DataMember(Name = "overridePath", EmitDefaultValue = true)]
        [DirectoryPath]
        [Editable]
        [Tokenizable]
        public string OverridePath { get; set; }
    }
}