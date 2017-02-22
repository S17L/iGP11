using iGP11.Library;
using iGP11.Tool.Domain.Configuration;

namespace iGP11.Tool.Domain.Model.TextureManagementSettings
{
    public class TextureManagementSettingsFactory
    {
        public TextureManagementSettings Create()
        {
            return Configurations.TextureManagementSettings.Deserialize<TextureManagementSettings>();
        }
    }
}