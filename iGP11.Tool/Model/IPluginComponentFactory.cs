using iGP11.Library.Component;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Model.InjectionSettings;

namespace iGP11.Tool.Model
{
    public interface IPluginComponentFactory
    {
        IComponent Create(InjectionSettings settings);

        IComponent Create(ProxyPluginSettings settings);
    }
}