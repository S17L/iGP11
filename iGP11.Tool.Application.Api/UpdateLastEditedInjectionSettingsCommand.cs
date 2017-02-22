using System.Runtime.Serialization;

using iGP11.Library.DDD;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class UpdateLastEditedInjectionSettingsCommand
    {
        public UpdateLastEditedInjectionSettingsCommand(AggregateId id)
        {
            Id = id;
        }

        [DataMember(Name = "id")]
        public AggregateId Id { get; private set; }
    }
}