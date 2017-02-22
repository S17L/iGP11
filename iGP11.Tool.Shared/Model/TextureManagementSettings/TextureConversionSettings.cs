using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.TextureManagementSettings
{
    [DataContract]
    [NoValidation]
    public class TextureConversionSettings
    {
        [ComponentName("ColorSpace")]
        [DataMember(Name = "colorSpace", EmitDefaultValue = true)]
        [Editable]
        [Order(0)]
        public Srgb ColorSpace { get; set; }

        [ComponentName("KeepMipmaps")]
        [DataMember(Name = "keepMipmaps", EmitDefaultValue = true)]
        public bool KeepMipmaps { get; set; }

        [ComponentName("OutputFormat")]
        [DataMember(Name = "outputFormat")]
        [Editable]
        [Order(1)]
        public DxgiFormat OutputFormat { get; set; }
    }
}