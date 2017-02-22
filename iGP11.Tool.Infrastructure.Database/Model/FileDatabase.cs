using System.Collections.Generic;
using System.Runtime.Serialization;

using iGP11.Library.DDD;
using iGP11.Tool.Domain.Model.ApplicationSettings;
using iGP11.Tool.Domain.Model.InjectionSettings;
using iGP11.Tool.Domain.Model.TextureManagementSettings;
using iGP11.Tool.Domain.Model.UsageStatistics;

namespace iGP11.Tool.Infrastructure.Database.Model
{
    [DataContract]
    public class FileDatabase
    {
        [DataMember(Name = "applicationSettings", IsRequired = true)]
        public ApplicationSettings ApplicationSettings { get; set; }

        [DataMember(Name = "injectionSettings", IsRequired = true)]
        public List<InjectionSettings> InjectionSettings { get; private set; } = new List<InjectionSettings>();

        [DataMember(Name = "lastEditedInjectionSettingsId")]
        public AggregateId LastEditedInjectionSettingsId { get; set; }

        [DataMember(Name = "textureConverterSettings", IsRequired = true)]
        public TextureManagementSettings TextureConverterSettings { get; set; }

        [DataMember(Name = "usageStatistics", IsRequired = true)]
        public UsageStatistics UsageStatistics { get; set; }
    }
}