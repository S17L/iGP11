using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;
using iGP11.Tool.Shared.Model.ApplicationSettings.Validation;

namespace iGP11.Tool.Shared.Model.ApplicationSettings
{
    [ApplicationSettingsValidator]
    [DataContract]
    public class ApplicationSettings
    {
        [ComponentName("ApplicationCommunicationPort")]
        [DataMember(Name = "applicationCommunicationPort", EmitDefaultValue = true)]
        [Editable]
        public ushort ApplicationCommunicationPort { get; set; }

        [ComponentName("ProxyCommunicationPort")]
        [DataMember(Name = "proxyCommunicationPort", EmitDefaultValue = true)]
        [Editable]
        public ushort ProxyCommunicationPort { get; set; }
    }
}