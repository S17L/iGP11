using System.Runtime.Serialization;

namespace iGP11.Tool.Application.Bootstrapper
{
    [DataContract]
    public class ConstantSettings
    {
        [DataMember(Name = "applicationCommunicationPort", IsRequired = true)]
        public ushort ApplicationCommunicationPort { get; set; }

        [DataMember(Name = "applicationListenerUri", IsRequired = true)]
        public string ApplicationListenerUri { get; set; }

        [DataMember(Name = "databaseEncryptionKey", IsRequired = true)]
        public string DatabaseEncryptionKey { get; set; }

        [DataMember(Name = "databaseFilePath", IsRequired = true)]
        public string DatabaseFilePath { get; set; }

        [DataMember(Name = "plugins", IsRequired = true)]
        public Plugins Plugins { get; set; }

        [DataMember(Name = "proxyCommunicationPort", EmitDefaultValue = true)]
        public ushort ProxyCommunicationPort { get; set; }

        [DataMember(Name = "systemIpAddress", IsRequired = true)]
        public string SystemIpAddress { get; set; }
    }
}