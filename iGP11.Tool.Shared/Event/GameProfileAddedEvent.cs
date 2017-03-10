using System;
using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class GameProfileAddedEvent
    {
        public GameProfileAddedEvent(Guid gameId, Guid gameProfileId, GameProfile gameProfile)
        {
            GameId = gameId;
            GameProfileId = gameProfileId;
            GameProfile = gameProfile;
        }

        [DataMember(Name = "gameId", IsRequired = true)]
        public Guid GameId { get; private set; }

        [DataMember(Name = "gameProfileId", IsRequired = true)]
        public Guid GameProfileId { get; private set; }

        [DataMember(Name = "gameProfile", IsRequired = true)]
        public GameProfile GameProfile { get; private set; }
    }
}