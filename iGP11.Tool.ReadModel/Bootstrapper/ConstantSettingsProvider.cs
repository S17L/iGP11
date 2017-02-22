using iGP11.Library;
using iGP11.Tool.ReadModel.Api.Model;

namespace iGP11.Tool.ReadModel.Bootstrapper
{
    public static class ConstantSettingsProvider
    {
        public static ConstantSettings Find()
        {
            return Configuration.ConstantSettings.Deserialize<ConstantSettings>();
        }
    }
}