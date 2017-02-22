using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Shared.Notification
{
    [DataContract]
    public class ProxyActivationStatusLoadedEvent
    {
        public ProxyActivationStatusLoadedEvent(ActivationStatus status)
        {
            Status = status;
        }

        [DataMember(Name = "state", IsRequired = true)]
        public ActivationStatus Status { get; private set; }
    }
}