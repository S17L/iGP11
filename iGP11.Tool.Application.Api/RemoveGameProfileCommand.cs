using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class RemoveGameProfileCommand
    {
        public RemoveGameProfileCommand(Guid id)
        {
            Id = id;
        }

        [DataMember(Name = "id")]
        public Guid Id { get; private set; }
    }
}