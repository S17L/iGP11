using System.Runtime.Serialization;

using iGP11.Library.DDD;

namespace iGP11.Tool.Domain.Model.ApplicationSettings
{
    [DataContract]
    public class ApplicationSettings : AggregateRoot<AggregateId>
    {
        public ApplicationSettings(AggregateId id, ushort applicationCommunicationPort, ushort proxyCommunicationPort)
            : base(id)
        {
            ApplicationCommunicationPort = applicationCommunicationPort;
            ProxyCommunicationPort = proxyCommunicationPort;
        }

        [DataMember(Name = "applicationCommunicationPort", IsRequired = true)]
        public ushort ApplicationCommunicationPort { get; set; }

        [DataMember(Name = "proxyCommunicationPort", IsRequired = true)]
        public ushort ProxyCommunicationPort { get; set; }
    }
}