using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class AddGameProfileCommand
    {
        public AddGameProfileCommand(string name, Guid gameId, Guid basedOnProfileId)
        {
            Name = name;
            GameId = gameId;
            BasedOnProfileId = basedOnProfileId;
        }

        [DataMember(Name = "gameId", IsRequired = true)]
        public Guid GameId { get; private set; }

        [DataMember(Name = "basedOnProfileId", IsRequired = true)]
        public Guid BasedOnProfileId { get; private set; }

        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; private set; }
    }
}