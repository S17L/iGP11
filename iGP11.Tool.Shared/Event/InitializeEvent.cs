using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Model.ApplicationSettings;
using iGP11.Tool.Shared.Model.GameSettings;
using iGP11.Tool.Shared.Model.TextureManagementSettings;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class InitializeEvent
    {
        public InitializeEvent(
            ApplicationSettings applicationSettings,
            IEnumerable<Game> games,
            Guid? lastEditedGameProfileId,
            TextureManagementSettings textureManagementSettings,
            UsageStatistics usageStatistics)
        {
            ApplicationSettings = applicationSettings;
            Games = games;
            LastEditedGameProfileId = lastEditedGameProfileId;
            TextureManagementSettings = textureManagementSettings;
            UsageStatistics = usageStatistics;
        }

        [DataMember(Name = "applicationSettings", IsRequired = true)]
        public ApplicationSettings ApplicationSettings { get; private set; }

        [DataMember(Name = "games", IsRequired = true)]
        public IEnumerable<Game> Games { get; private set; }

        [DataMember(Name = "lastEditedGameProfileId", IsRequired = true)]
        public Guid? LastEditedGameProfileId { get; private set; }

        [DataMember(Name = "textureManagementSettings", IsRequired = true)]
        public TextureManagementSettings TextureManagementSettings { get; private set; }

        [DataMember(Name = "usageStatistics", IsRequired = true)]
        public UsageStatistics UsageStatistics { get; private set; }
    }
}