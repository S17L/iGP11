using System;
using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.InjectionSettings
{
    [DataContract]
    public class InjectionSettings
    {
        [DataMember(Name = "applicationFilePath", EmitDefaultValue = true)]
        [Editable]
        [FilePath]
        public string ApplicationFilePath { get; set; }

        [DataMember(Name = "configurationDirectoryPath", EmitDefaultValue = true)]
        [Editable]
        [Tokenizable]
        public string ConfigurationDirectoryPath { get; set; }

        [Complex]
        [DataMember(Name = "direct3D11Settings")]
        [Editable]
        public Direct3D11Settings Direct3D11Settings { get; set; }

        [DataMember(Name = "establishCommunication")]
        [Editable]
        public bool EstablishCommunication { get; set; }

        [DataMember(Name = "id", EmitDefaultValue = true)]
        public Guid Id { get; set; }

        [DataMember(Name = "logsDirectoryPath", IsRequired = true)]
        [Editable]
        [Tokenizable]
        public string LogsDirectoryPath { get; set; }

        [DataMember(Name = "name", EmitDefaultValue = true)]
        [Editable]
        public string Name { get; set; }

        [DataMember(Name = "pluginType", EmitDefaultValue = true)]
        [Editable]
        public PluginType PluginType { get; set; }
    }
}