using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Model
{
    [DataContract]
    public class ProxySettings : ProxyPluginSettings
    {
        [DataMember(Name = "activationStatus", IsRequired = true)]
        public ActivationStatus ActivationStatus { get; set; }

        [DataMember(Name = "gameFilePath", IsRequired = true)]
        public string GameFilePath { get; set; }

        [DataMember(Name = "logsDirectoryPath", IsRequired = true)]
        public string LogsDirectoryPath { get; set; }

        [DataMember(Name = "proxyDirectoryPath", IsRequired = true)]
        public string ProxyDirectoryPath { get; set; }
    }
}