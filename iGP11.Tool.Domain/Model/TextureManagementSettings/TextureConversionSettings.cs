using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Domain.Model.TextureManagementSettings
{
    [DataContract]
    public class TextureConversionSettings
    {
        [DataMember(Name = "colorSpace", EmitDefaultValue = true)]
        public Srgb ColorSpace { get; set; }

        [DataMember(Name = "keepMipmaps", EmitDefaultValue = true)]
        public bool KeepMipmaps { get; set; }

        [DataMember(Name = "outputFormat")]
        public DxgiFormat OutputFormat { get; set; }
    }
}