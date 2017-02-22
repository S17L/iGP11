using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;
using iGP11.Library.DDD;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Domain.Model.InjectionSettings
{
    [DataContract]
    public class InjectionSettings : AggregateRoot<AggregateId>
    {
        public InjectionSettings(
            AggregateId id,
            string name,
            string applicationFilePath,
            string configurationDirectoryPath,
            string logsDirectoryPath,
            PluginType pluginType,
            bool establishCommunication,
            Direct3D11Settings direct3D11Settings)
            : base(id)
        {
            Name = name;
            ApplicationFilePath = applicationFilePath;
            ConfigurationDirectoryPath = configurationDirectoryPath;
            LogsDirectoryPath = logsDirectoryPath;
            PluginType = pluginType;
            EstablishCommunication = establishCommunication;
            Direct3D11Settings = direct3D11Settings;
        }

        [DataMember(Name = "applicationFilePath", IsRequired = true)]
        [Editable]
        [FilePath]
        public string ApplicationFilePath { get; set; }

        [DataMember(Name = "configurationDirectoryPath", IsRequired = true)]
        [DirectoryPath]
        [Editable]
        [Tokenizable]
        public string ConfigurationDirectoryPath { get; set; }

        [Complex]
        [DataMember(Name = "direct3D11Settings", IsRequired = true)]
        [Editable]
        public Direct3D11Settings Direct3D11Settings { get; set; }

        [DataMember(Name = "establishCommunication", IsRequired = true)]
        [Editable]
        public bool EstablishCommunication { get; set; }

        [DataMember(Name = "logsDirectoryPath", IsRequired = true)]
        [DirectoryPath]
        [Editable]
        [Tokenizable]
        public string LogsDirectoryPath { get; set; }

        [DataMember(Name = "name", IsRequired = true)]
        [Editable]
        public string Name { get; set; }

        [DataMember(Name = "pluginType", IsRequired = true)]
        [Editable]
        public PluginType PluginType { get; set; }
    }
}