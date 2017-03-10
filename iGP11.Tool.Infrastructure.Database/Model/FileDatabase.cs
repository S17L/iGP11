using System.Collections.Generic;
using System.Runtime.Serialization;

using iGP11.Library.DDD;
using iGP11.Tool.Domain.Model.ApplicationSettings;
using iGP11.Tool.Domain.Model.GameSettings;
using iGP11.Tool.Domain.Model.TextureManagementSettings;
using iGP11.Tool.Domain.Model.UsageStatistics;

namespace iGP11.Tool.Infrastructure.Database.Model
{
    [DataContract]
    public class FileDatabase
    {
        [DataMember(Name = "applicationSettings", IsRequired = true)]
        public ApplicationSettings ApplicationSettings { get; set; }

        [DataMember(Name = "games", IsRequired = true)]
        public List<Game> Games { get; private set; } = new List<Game>();

        [DataMember(Name = "lastEditedProfileId")]
        public AggregateId LastEditedProfileId { get; set; }

        [DataMember(Name = "textureConverterSettings", IsRequired = true)]
        public TextureManagementSettings TextureConverterSettings { get; set; }

        [DataMember(Name = "usageStatistics", IsRequired = true)]
        public UsageStatistics UsageStatistics { get; set; }
    }
}