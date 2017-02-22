using System.Runtime.Serialization;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class UpdateApplicationSettingsCommand
    {
        public UpdateApplicationSettingsCommand(ushort applicationCommunicationPort, ushort proxyCommunicationPort)
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