using System.IO;

using iGP11.Library;

namespace iGP11.Tool.Application.Bootstrapper
{
    public static class ConstantSettingsProvider
    {
        public static ConstantSettings Find()
        {
            var settings = Configuration.ConstantSettings.Deserialize<ConstantSettings>();
            settings.Plugins.Direct3D11 = Path.GetFullPath(settings.Plugins.Direct3D11);
            settings.Plugins.Proxy = Path.GetFullPath(settings.Plugins.Proxy);

            return settings;
        }
    }
}