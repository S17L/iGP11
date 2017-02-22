using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class UpdateProxySettingsCommand
    {
        public UpdateProxySettingsCommand(ProxyPluginSettings proxyPluginSettings)
        {
            ProxyPluginSettings = proxyPluginSettings;
        }

        [DataMember(Name = "proxyPluginSettings", IsRequired = true)]
        public ProxyPluginSettings ProxyPluginSettings { get; private set; }
    }
}