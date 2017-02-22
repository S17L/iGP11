using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Shared.Notification
{
    [DataContract]
    public class ApplicationStartedEvent
    {
        public ApplicationStartedEvent(InjectionStatus status)
        {
            Status = status;
        }

        [DataMember(Name = "status", IsRequired = true)]
        public InjectionStatus Status { get; private set; }
    }
}