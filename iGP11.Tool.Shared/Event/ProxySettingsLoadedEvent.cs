using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class ProxySettingsLoadedEvent
    {
        public ProxySettingsLoadedEvent(ProxySettings proxySettings)
        {
            ProxySettings = proxySettings;
        }

        [DataMember(Name = "proxySettings", IsRequired = true)]
        public ProxySettings ProxySettings { get; private set; }
    }
}