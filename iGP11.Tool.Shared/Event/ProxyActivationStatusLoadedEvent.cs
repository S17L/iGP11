using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class ProxyActivationStatusLoadedEvent
    {
        public ProxyActivationStatusLoadedEvent(string applicationFilePath, ActivationStatus activationStatus)
        {
            ApplicationFilePath = applicationFilePath;
            ActivationStatus = activationStatus;
        }

        [DataMember(Name = "activationStatus", IsRequired = true)]
        public ActivationStatus ActivationStatus { get; private set; }

        [DataMember(Name = "applicationFilePath", IsRequired = true)]
        public string ApplicationFilePath { get; private set; }
    }
}