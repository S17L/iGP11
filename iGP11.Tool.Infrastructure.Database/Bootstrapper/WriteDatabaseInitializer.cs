using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using iGP11.Library;
using iGP11.Library.DDD;
using iGP11.Tool.Domain;
using iGP11.Tool.Domain.Exceptions;
using iGP11.Tool.Domain.Model.ApplicationSettings;
using iGP11.Tool.Domain.Model.InjectionSettings;
using iGP11.Tool.Domain.Model.TextureManagementSettings;
using iGP11.Tool.Domain.Model.UsageStatistics;
using iGP11.Tool.Infrastructure.Database.Model;
using iGP11.Tool.Infrastructure.Database.Repository;

namespace iGP11.Tool.Infrastructure.Database.Bootstrapper
{
    public static class WriteDatabaseInitializer
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public static FileDatabaseContext DatabaseContext { get; private set; }

        public static async Task Initialize(WriteDatabaseConfiguration configuration)
        {
            await _semaphore.WaitAsync();

            try
            {
                var databaseFilePath = Path.Combine(configuration.DirectoryPath, configuration.FileName);
                var encryptionService = new EncryptionService(new SecuredText(configuration.EncryptionKey));

                var databaseContext = new FileDatabaseContext(databaseFilePath, encryptionService);
                databaseContext.Initialize();

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await AddApplicationSettings(databaseContext);
                    await AddTextureConverterSettings(databaseContext);
                    await AddUsageStatistics(databaseContext);

                    if (databaseContext.InjectionSettings.IsNullOrEmpty())
                    {
                        foreach (Direct3D11ProfileType profileType in Enum.GetValues(typeof(Direct3D11ProfileType)))
                        {
                            try
                            {
                                await AddInjectionSettings(profileType, databaseContext);
                            }
                            catch (ProfileTemplateNotFoundException)
                            {
                            }
                        }

                        var profile = (Direct3D11ProfileType)typeof(Direct3D11ProfileType).GetDefaultValue().GetValueOrDefault();
                        await new InjectionSettingsRepository(databaseContext).ChangeDefaultAsync(profile.GetAggregateId());
                    }

                    transaction.Complete();
                }

                DatabaseContext = databaseContext;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private static async Task AddApplicationSettings(FileDatabaseContext context)
        {
            var repository = new ApplicationSettingsRepository(context);
            try
            {
                await repository.LoadAsync();
            }
            catch (AggregateRootNotFoundException)
            {
                await repository.SaveAsync(new ApplicationSettingsFactory().Create());
            }
        }

        private static async Task AddInjectionSettings(Direct3D11ProfileType profileType, FileDatabaseContext context)
        {
            var aggregateId = profileType.GetAggregateId();
            if (aggregateId == null)
            {
                return;
            }

            var repository = new InjectionSettingsRepository(context);
            try
            {
                await repository.LoadAsync(aggregateId);
            }
            catch (AggregateRootNotFoundException)
            {
                await repository.SaveAsync(new InjectionSettingsFactory().Create(profileType));
            }
        }

        private static async Task AddTextureConverterSettings(FileDatabaseContext context)
        {
            var repository = new TextureManagementSettingsRepository(context);
            try
            {
                await repository.LoadAsync();
            }
            catch (AggregateRootNotFoundException)
            {
                await repository.SaveAsync(new TextureManagementSettingsFactory().Create());
            }
        }

        private static async Task AddUsageStatistics(FileDatabaseContext context)
        {
            var repository = new UsageStatisticsRepository(context);
            try
            {
                await repository.LoadAsync();
            }
            catch (AggregateRootNotFoundException)
            {
                await repository.SaveAsync(new UsageStatisticsFactory().Create());
            }
        }
    }
}