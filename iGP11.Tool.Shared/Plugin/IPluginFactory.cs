using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.Shared.Plugin
{
    public interface IPluginFactory
    {
        IPluginDataAccessLayer Create(GamePackage package);

        IPluginDataAccessLayer Create(ProxyPluginSettings settings);
    }
}