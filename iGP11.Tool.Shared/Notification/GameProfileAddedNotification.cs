using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Notification
{
    [DataContract]
    public class GameProfileAddedNotification
    {
        public GameProfileAddedNotification(Guid id)
        {
            Id = id;
        }

        [DataMember(Name = "id", IsRequired = true)]
        public Guid Id { get; private set; }
    }
}