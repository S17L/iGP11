using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Model
{
    [DataContract]
    public class TextureMetadata
    {
        [DataMember(Name = "dxgiFormat", IsRequired = true)]
        public DxgiFormat Format { get; set; }

        [DataMember(Name = "height", IsRequired = true)]
        public ulong Height { get; set; }

        [DataMember(Name = "mipmapsCount", IsRequired = true)]
        public ulong MipmapsCount { get; set; }

        [DataMember(Name = "width", IsRequired = true)]
        public ulong Width { get; set; }
    }
}