using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;
using iGP11.Library.DDD;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Domain.Model.GameSettings
{
    [DataContract]
    public class GameProfile : Entity<AggregateId>
    {
        public GameProfile(
            AggregateId id,
            AggregateId gameId,
            string name,
            string proxyDirectoryPath,
            string logsDirectoryPath,
            PluginType pluginType,
            Direct3D11Settings direct3D11Settings)
            : base(id)
        {
            GameId = gameId;
            Name = name;
            ProxyDirectoryPath = proxyDirectoryPath;
            LogsDirectoryPath = logsDirectoryPath;
            PluginType = pluginType;
            Direct3D11Settings = direct3D11Settings;
        }

        [DataMember(Name = "gameId", IsRequired = true)]
        public AggregateId GameId { get; private set; }

        [DataMember(Name = "name", IsRequired = true)]
        [Editable]
        public string Name { get; set; }

        [Complex]
        [DataMember(Name = "direct3D11Settings", IsRequired = true)]
        [Editable]
        public Direct3D11Settings Direct3D11Settings { get; set; }

        [DataMember(Name = "logsDirectoryPath", IsRequired = true)]
        [DirectoryPath]
        [Editable]
        [Tokenizable]
        public string LogsDirectoryPath { get; set; }

        [DataMember(Name = "pluginType", IsRequired = true)]
        [Editable]
        public PluginType PluginType { get; set; }

        [DataMember(Name = "proxyDirectoryPath", IsRequired = true)]
        [DirectoryPath]
        [Editable]
        [Tokenizable]
        public string ProxyDirectoryPath { get; set; }
    }
}