using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class AddGameProfileCommand
    {
        public AddGameProfileCommand(string name, Guid gameId, Guid basedOnGameProfileId)
        {
            Name = name;
            GameId = gameId;
            BasedOnGameProfileId = basedOnGameProfileId;
        }

        [DataMember(Name = "basedOnGameProfileId", IsRequired = true)]
        public Guid BasedOnGameProfileId { get; private set; }

        [DataMember(Name = "gameId", IsRequired = true)]
        public Guid GameId { get; private set; }

        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; private set; }
    }
}