using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class LastEditedGameProfileUpdatedEvent
    {
        public LastEditedGameProfileUpdatedEvent(Guid id)
        {
            Id = id;
        }

        [DataMember(Name = "id", IsRequired = true)]
        public Guid Id { get; private set; }
    }
}