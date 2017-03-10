using iGP11.Library.Component;
using iGP11.Tool.ReadModel.Api.Model;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Model
{
    public interface IPluginComponentFactory
    {
        IComponent Create(GamePackage package);

        IComponent Create(ProxyPluginSettings settings);
    }
}