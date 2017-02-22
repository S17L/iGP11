using System.Runtime.Serialization;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client.Action
{
    [DataContract]
    public class Event<TData>
    {
        public Event(TData data, EndpointId notificationRecipientId)
        {
            Data = data;
            NotificationRecipientId = notificationRecipientId;
        }

        [DataMember(Name = "data", IsRequired = true)]
        public TData Data { get; private set; }

        [DataMember(Name = "notificationRecipientId", IsRequired = true)]
        public EndpointId NotificationRecipientId { get; private set; }
    }
}