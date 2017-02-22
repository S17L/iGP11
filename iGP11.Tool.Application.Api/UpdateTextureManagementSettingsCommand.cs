using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model.TextureManagementSettings;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class UpdateTextureManagementSettingsCommand
    {
        public UpdateTextureManagementSettingsCommand(TextureManagementSettings settings)
        {
            Settings = settings;
        }

        [DataMember(Name = "settings", IsRequired = true)]
        public TextureManagementSettings Settings { get; private set; }
    }
}