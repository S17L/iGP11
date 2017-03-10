using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Shared.Notification
{
    [DataContract]
    public class GeneratedTexturePreviewNotification
    {
        public GeneratedTexturePreviewNotification(Texture texture, TextureMetadata textureMetadata)
        {
            Texture = texture;
            TextureMetadata = textureMetadata;
        }

        [DataMember(Name = "texture", IsRequired = true)]
        public Texture Texture { get; private set; }

        [DataMember(Name = "textureMetadata", IsRequired = true)]
        public TextureMetadata TextureMetadata { get; private set; }
    }
}