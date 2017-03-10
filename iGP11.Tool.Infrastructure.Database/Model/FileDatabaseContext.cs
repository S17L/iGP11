using System.Collections.Generic;

using iGP11.Library;
using iGP11.Library.DDD;
using iGP11.Library.File;
using iGP11.Tool.Domain.Model.ApplicationSettings;
using iGP11.Tool.Domain.Model.GameSettings;
using iGP11.Tool.Domain.Model.TextureManagementSettings;
using iGP11.Tool.Domain.Model.UsageStatistics;

namespace iGP11.Tool.Infrastructure.Database.Model
{
    public class FileDatabaseContext : DatabaseContext<FileDatabase>
    {
        private readonly IEncryptionService _encryptionService;

        public FileDatabaseContext(string filePath, IEncryptionService encryptionService)
            : base(filePath)
        {
            _encryptionService = encryptionService;
        }

        public ApplicationSettings ApplicationSettings
        {
            get { return Database.ApplicationSettings; }
            set { Database.ApplicationSettings = value; }
        }

        public ICollection<Game> Games => Database.Games;

        public AggregateId LastEditedProfileId
        {
            get { return Database.LastEditedProfileId; }
            set { Database.LastEditedProfileId = value; }
        }

        public TextureManagementSettings TextureConverterSettings
        {
            get { return Database.TextureConverterSettings; }
            set { Database.TextureConverterSettings = value; }
        }

        public UsageStatistics UsageStatistics
        {
            get { return Database.UsageStatistics; }
            set { Database.UsageStatistics = value; }
        }

        protected override void OnDatabaseInitializing(DatabaseConfigurationBuilder<FileDatabase> configurator)
        {
            configurator.Configure(new DatabaseEncryption(_encryptionService));
            configurator.Configure(new NewDatabaseFactory<FileDatabase>());
            configurator.Configure(new DataContractDatabaseSerializer<FileDatabase>());
        }
    }
}