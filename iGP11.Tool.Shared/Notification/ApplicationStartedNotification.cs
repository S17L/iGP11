using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Shared.Notification
{
    [DataContract]
    public class ApplicationStartedNotification
    {
        public ApplicationStartedNotification(GameLaunchingStatus status)
        {
            Status = status;
        }

        [DataMember(Name = "status", IsRequired = true)]
        public GameLaunchingStatus Status { get; private set; }
    }
}