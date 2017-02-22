using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model.InjectionSettings;

namespace iGP11.Tool.Shared.Event
{
    [DataContract]
    public class InjectionSettingsUpdatedEvent
    {
        public InjectionSettingsUpdatedEvent(InjectionSettings injectionSettings)
        {
            InjectionSettings = injectionSettings;
        }

        [DataMember(Name = "injectionSettings", IsRequired = true)]
        public InjectionSettings InjectionSettings { get; private set; }
    }
}