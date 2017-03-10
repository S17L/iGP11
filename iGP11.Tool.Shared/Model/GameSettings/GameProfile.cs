using System;
using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [DataContract]
    public class GameProfile
    {
        [Complex]
        [DataMember(Name = "direct3D11Settings")]
        [Editable]
        public Direct3D11Settings Direct3D11Settings { get; set; }

        [DataMember(Name = "gameId", EmitDefaultValue = true)]
        public Guid GameId { get; set; }

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

        [DataMember(Name = "proxyDirectoryPath", EmitDefaultValue = true)]
        [Editable]
        [Tokenizable]
        public string ProxyDirectoryPath { get; set; }
    }
}