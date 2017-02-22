using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Model
{
    [DataContract]
    public class ProxySettings : ProxyPluginSettings
    {
        [DataMember(Name = "activationStatus", IsRequired = true)]
        public ActivationStatus ActivationStatus { get; set; }

        [DataMember(Name = "applicationFilePath", IsRequired = true)]
        public string ApplicationFilePath { get; set; }

        [DataMember(Name = "configurationDirectoryPath", IsRequired = true)]
        public string ConfigurationDirectoryPath { get; set; }

        [DataMember(Name = "logsDirectoryPath", IsRequired = true)]
        public string LogsDirectoryPath { get; set; }
    }
}