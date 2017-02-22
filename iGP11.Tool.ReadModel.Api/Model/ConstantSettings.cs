using System.Runtime.Serialization;

namespace iGP11.Tool.ReadModel.Api.Model
{
    [DataContract]
    public class ConstantSettings
    {
        [DataMember(Name = "applicationCommunicationPort", IsRequired = true)]
        public ushort ApplicationCommunicationPort { get; set; }

        [DataMember(Name = "feedbackEmail", IsRequired = true)]
        public string FeedbackEmail { get; set; }

        [DataMember(Name = "proxyCommunicationPort", EmitDefaultValue = true)]
        public ushort ProxyCommunicationPort { get; set; }

        [DataMember(Name = "usedIconsUri", IsRequired = true)]
        public string UsedIconsUri { get; set; }
    }
}