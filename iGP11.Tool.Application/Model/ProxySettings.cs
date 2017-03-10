using System.Runtime.Serialization;

using iGP11.Tool.Domain.Model.GameSettings;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Application.Model
{
    [DataContract]
    public class ProxySettings
    {
        [DataMember(Name = "activationStatus", IsRequired = true)]
        public ActivationStatus ActivationStatus { get; set; }

        [DataMember(Name = "direct3D11Settings", IsRequired = true)]
        public Direct3D11Settings Direct3D11Settings { get; set; }

        [DataMember(Name = "gameFilePath", IsRequired = true)]
        public string GameFilePath { get; set; }

        [DataMember(Name = "logsDirectoryPath", IsRequired = true)]
        public string LogsDirectoryPath { get; set; }

        [DataMember(Name = "pluginType", IsRequired = true)]
        public PluginType PluginType { get; set; }

        [DataMember(Name = "proxyDirectoryPath", IsRequired = true)]
        public string ProxyDirectoryPath { get; set; }
    }
}