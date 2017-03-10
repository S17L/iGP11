using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class StarGameCommand
    {
        public StarGameCommand(Guid gameId)
        {
            GameId = gameId;
        }

        [DataMember(Name = "gameId")]
        public Guid GameId { get; private set; }
    }
}