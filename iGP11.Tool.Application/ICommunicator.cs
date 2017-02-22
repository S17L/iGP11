using System;
using System.Threading.Tasks;

using iGP11.Tool.Shared.Model;

using ApplicationModel = iGP11.Tool.Application.Model;

namespace iGP11.Tool.Application
{
    public interface ICommunicator : IDisposable
    {
        Task<ApplicationModel.CommunicationResult<ActivationStatus>> GetActivationStatusAsync(PluginType pluginType);

        Task<ApplicationModel.CommunicationResult<ApplicationModel.ProxySettings>> GetProxySettingsAsync();

        Task<bool> UpdateAsync(ApplicationModel.ProxyPluginSettings settings);
    }
}