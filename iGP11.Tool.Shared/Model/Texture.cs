using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Model
{
    [DataContract]
    public class Texture
    {
        [DataMember(Name = "buffer", IsRequired = true)]
        public byte[] Buffer { get; set; }

        [DataMember(Name = "metadata", IsRequired = true)]
        public TextureMetadata Metadata { get; set; }
    }
}