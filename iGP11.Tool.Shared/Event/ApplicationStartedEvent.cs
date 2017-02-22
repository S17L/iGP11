using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class ApplicationStartedEvent
    {
        public ApplicationStartedEvent(string applicationFilePath, InjectionStatus status)
        {
            ApplicationFilePath = applicationFilePath;
            Status = status;
        }

        [DataMember(Name = "applicationFilePath", IsRequired = true)]
        public string ApplicationFilePath { get; private set; }

        [DataMember(Name = "status", IsRequired = true)]
        public InjectionStatus Status { get; private set; }
    }
}