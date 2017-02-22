using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model.InjectionSettings;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class UpdateInjectionSettingsCommand
    {
        public UpdateInjectionSettingsCommand(InjectionSettings injectionSettings)
        {
            InjectionSettings = injectionSettings;
        }

        [DataMember(Name = "injectionSettings")]
        public InjectionSettings InjectionSettings { get; private set; }
    }
}