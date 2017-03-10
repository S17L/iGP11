using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Shared.Notification
{
    [DataContract]
    public class ProxyActivationStatusLoadedNotification
    {
        public ProxyActivationStatusLoadedNotification(ActivationStatus status)
        {
            Status = status;
        }

        [DataMember(Name = "state", IsRequired = true)]
        public ActivationStatus Status { get; private set; }
    }
}