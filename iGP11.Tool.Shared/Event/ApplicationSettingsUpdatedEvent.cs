using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class ApplicationSettingsUpdatedEvent
    {
        public ApplicationSettingsUpdatedEvent(ushort applicationCommunicationPort, ushort proxyCommunicationPort)
        {
            ApplicationCommunicationPort = applicationCommunicationPort;
            ProxyCommunicationPort = proxyCommunicationPort;
        }

        [DataMember(Name = "applicationCommunicationPort")]
        public ushort ApplicationCommunicationPort { get; private set; }

        [DataMember(Name = "proxyCommunicationPort")]
        public ushort ProxyCommunicationPort { get; private set; }
    }
}