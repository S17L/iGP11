using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class GameProfileRemovedEvent
    {
        public GameProfileRemovedEvent(Guid lastEditedGameProfileId, Guid removedGameProfileId)
        {
            LastEditedGameProfileId = lastEditedGameProfileId;
            RemovedGameProfileId = removedGameProfileId;
        }

        [DataMember(Name = "lastEditedGameProfileId", IsRequired = true)]
        public Guid LastEditedGameProfileId { get; private set; }

        [DataMember(Name = "removedGameProfileId", IsRequired = true)]
        public Guid RemovedGameProfileId { get; private set; }
    }
}