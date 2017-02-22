using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class AddInjectionSettingsCommand
    {
        public AddInjectionSettingsCommand(Guid basedOnInjectionSettingsId, string name)
        {
            Name = name;
            BasedOnInjectionSettingsId = basedOnInjectionSettingsId;
        }

        [DataMember(Name = "basedOnInjectionSettingsId")]
        public Guid BasedOnInjectionSettingsId { get; private set; }

        [DataMember(Name = "name")]
        public string Name { get; private set; }
    }
}