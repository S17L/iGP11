using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Autofac;

using iGP11.Library;
using iGP11.Library.Component;
using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Library.EventPublisher;
using iGP11.Library.Hub;
using iGP11.Library.Hub.Client;
using iGP11.Library.Hub.Client.Action;
using iGP11.Library.Hub.Model;
using iGP11.Library.Hub.Queue;
using iGP11.Library.Hub.Shared;
using iGP11.Library.Hub.Transport;
using iGP11.Library.Network;
using iGP11.Library.Scheduler;
using iGP11.Tool.Application;
using iGP11.Tool.Bootstrapper.Autofac;
using iGP11.Tool.Bootstrapper.AutoMapper;
using iGP11.Tool.Bootstrapper.Log4net;
using iGP11.Tool.Domain.Model.Directory;
using iGP11.Tool.Domain.Model.GameSettings;
using iGP11.Tool.Infrastructure.Communication;
using iGP11.Tool.Infrastructure.Database.Bootstrapper;
using iGP11.Tool.Infrastructure.Database.Repository;
using iGP11.Tool.Localization;
using iGP11.Tool.ReadModel.Bootstrapper;

using ApplicationBootstrapper = iGP11.Tool.Application.Bootstrapper;

namespace iGP11.Tool.Bootstrapper
{
    public static class Bootstrapper
    {
        private const int Interval = 20000;

        private static readonly BlockingTaskQueue _queue = new BlockingTaskQueue();

        private static Hub _hub;
        private static bool _initialized;
        private static IScheduler _scheduler;

        public static async Task PreStart()
        {
            using (await _queue.GetBlockingScope())
            {
                Localization.Localization.Current = new ResourceLocalizationAdapter(
                    new AutoFormattedMissingTextProvider(),
                    new PassThroughTextProcessor());

                Logger.Current = new Log4NetAdapter();
                Mapper.Current = new AutoMapperAdapter();
            }
        }

        public static async Task StartAsync()
        {
            using (await _queue.GetBlockingScope())
            {
                if (_initialized)
                {
                    throw new InvalidOperationException("Bootstrapper has already been invoked");
                }

                var constantSettings = ApplicationBootstrapper.ConstantSettingsProvider.Find();
                var directoryPath = AppDomain.CurrentDomain.BaseDirectory;

                await WriteDatabaseInitializer.Initialize(new WriteDatabaseConfiguration(directoryPath, constantSettings.DatabaseFilePath, constantSettings.DatabaseEncryptionKey));
                await ReadDatabaseInitializer.Initialize();

                var applicationSettings = await new ApplicationSettingsRepository(WriteDatabaseInitializer.DatabaseContext).LoadAsync();
                var applicationListenerUri = string.Format(constantSettings.ApplicationListenerUri, constantSettings.SystemIpAddress, applicationSettings.ApplicationCommunicationPort);

                var builder = new ContainerBuilder();

                var assemblies = new[] { Assembly.GetEntryAssembly() }
                    .Union(new[]
                        {
                            "iGP11.Library.dll",
                            "iGP11.Library.Component.dll",
                            "iGP11.Library.Component.DataAnnotations.dll",
                            "iGP11.Library.DDD.dll",
                            "iGP11.Library.EventPublisher.dll",
                            "iGP11.Library.File.dll",
                            "iGP11.Library.Hub.dll",
                            "iGP11.Library.Hub.Client.dll",
                            "iGP11.Library.Hub.Shared.dll",
                            "iGP11.Library.Hub.Transport.dll",
                            "iGP11.Library.Network.dll",
                            "iGP11.Library.Scheduler.dll",
                            "iGP11.Tool.Application.dll",
                            "iGP11.Tool.Application.Api.dll",
                            "iGP11.Tool.Domain.dll",
                            "iGP11.Tool.Infrastructure.Communication.dll",
                            "iGP11.Tool.Infrastructure.External.dll",
                            "iGP11.Tool.Infrastructure.Database.dll",
                            "iGP11.Tool.Localization.dll",
                            "iGP11.Tool.ReadModel.dll",
                            "iGP11.Tool.ReadModel.Api.dll",
                            "iGP11.Tool.Shared.dll"
                        }
                        .Select(Assembly.LoadFrom))
                    .ToArray();

                foreach (var assembly in assemblies)
                {
                    builder.RegisterAssemblyTypes(assembly)
                        .AsSelf()
                        .AsImplementedInterfaces();
                }

                builder.Register(
                        context =>
                        {
                            var listener = new NetworkListener($"{applicationListenerUri}/", context.Resolve<ILogger>());
                            foreach (dynamic handler in assemblies.GetImplementations(typeof(INetworkCommandHandler)).Select(context.Resolve))
                            {
                                listener.Register(handler);
                            }

                            return listener;
                        })
                    .SingleInstance()
                    .AsSelf()
                    .AsImplementedInterfaces();

                var proxyCommunicationPort = applicationSettings.ProxyCommunicationPort;
                var systemIpAddress = constantSettings.SystemIpAddress;

                var eventPublisher = new EventPublisher();
                _scheduler = new BlockingScheduler(eventPublisher.Collect, Interval);
                _scheduler.Start();

                builder.Register(context => Logger.Current).SingleInstance().AsSelf().AsImplementedInterfaces();
                builder.Register(context => WriteDatabaseInitializer.DatabaseContext).SingleInstance().AsSelf().AsImplementedInterfaces();
                builder.Register(context => ReadDatabaseInitializer.Database).SingleInstance().AsSelf().AsImplementedInterfaces();
                builder.Register(context => new CommunicatorFactory(systemIpAddress, proxyCommunicationPort, Logger.Current)).AsSelf().AsImplementedInterfaces();
                builder.Register(context => eventPublisher).SingleInstance().AsSelf().AsImplementedInterfaces();
                builder.Register(context => constantSettings.Plugins).AsSelf().AsImplementedInterfaces();
                builder.Register(context => new NetworkPublisher(applicationListenerUri)).AsSelf().AsImplementedInterfaces();

                builder.Register(context => new GameSettingsProcessWatcher(
                        constantSettings.Plugins,
                        systemIpAddress,
                        proxyCommunicationPort,
                        context.Resolve<ComponentAssembler>(),
                        context.Resolve<IDirectoryRepository>(),
                        context.Resolve<IGameRepository>(),
                        context.Resolve<IInjectionService>(),
                        Logger.Current,
                        context.Resolve<IProcessWatcher>()))
                    .SingleInstance()
                    .AsSelf()
                    .AsImplementedInterfaces();

                var hubEventRepository = new InMemoryEventRepository<HubEvent>();
                var endpointEventRepository = new InMemoryEventRepository<EndpointEvent>();
                var transport = new InMemoryTransport();
                var eventSerializerFactory = new DataContractEventSerializerFactory();
                var hubClientFactory = new HubClientFactory(transport, eventSerializerFactory);

                _hub = new Hub(hubEventRepository, endpointEventRepository, transport, Logger.Current);
                var commandEndpointId = EndpointId.Generate();
                var queryEndpointId = EndpointId.Generate();

                builder.Register(context => new DomainActionBuilder(new ActionBuilder(hubClientFactory, eventSerializerFactory), commandEndpointId)).AsSelf().AsImplementedInterfaces();
                DependencyResolver.Current = new AutofacResolver(builder.Build());

                var domainCommandHubHost = new DomainCommandHubHost(commandEndpointId, queryEndpointId, hubClientFactory);
                foreach (dynamic handler in assemblies.GetImplementations(typeof(IDomainCommandHandler<>)).Select(DependencyResolver.Current.Resolve))
                {
                    ListenFor(domainCommandHubHost, handler);
                }

                var domainEventHubHost = new DomainEventHubHost(queryEndpointId, hubClientFactory);
                foreach (dynamic handler in assemblies.GetImplementations(typeof(IDomainEventHandler<>)).Select(DependencyResolver.Current.Resolve))
                {
                    ListenFor(domainEventHubHost, handler);
                }

                domainCommandHubHost.Start();
                domainEventHubHost.Start();

                _initialized = true;
            }
        }

        private static void ListenFor<TCommand>(DomainCommandHubHost host, IDomainCommandHandler<TCommand> handler)
        {
            host.ListenFor(handler, new DataContractEventSerializer<Event<TCommand>>());
        }

        private static void ListenFor<TEvent>(DomainEventHubHost host, IDomainEventHandler<TEvent> handler)
        {
            host.ListenFor(handler, new DataContractEventSerializer<Event<TEvent>>());
        }
    }
}