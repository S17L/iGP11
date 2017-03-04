using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.Component;
using iGP11.Library.DDD;
using iGP11.Tool.Application.Bootstrapper;
using iGP11.Tool.Domain.Model.Directory;
using iGP11.Tool.Domain.Model.InjectionSettings;

namespace iGP11.Tool.Application
{
    public class InjectionSettingsProcessWatcher
    {
        private readonly ComponentAssembler _assembler;
        private readonly string _communicationAddress;
        private readonly ushort _communicationPort;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IInjectionService _injectionService;
        private readonly IInjectionSettingsRepository _injectionSettingRepository;
        private readonly ILogger _logger;
        private readonly Plugins _plugins;
        private readonly IProcessWatcher _processWatcher;
        private readonly BlockingTaskQueue _queue = new BlockingTaskQueue();
        private readonly IDictionary<InjectionSettingsWatchableProcess, IDisposable> _watchableProcesses = new Dictionary<InjectionSettingsWatchableProcess, IDisposable>();

        public InjectionSettingsProcessWatcher(
            Plugins plugins,
            string communicationAddress,
            ushort communicationPort,
            ComponentAssembler assembler,
            IDirectoryRepository directoryRepository,
            IInjectionService injectionService,
            IInjectionSettingsRepository injectionSettingsRepository,
            ILogger logger,
            IProcessWatcher processWatcher)
        {
            _plugins = plugins;
            _communicationAddress = communicationAddress;
            _communicationPort = communicationPort;
            _assembler = assembler;
            _directoryRepository = directoryRepository;
            _injectionService = injectionService;
            _injectionSettingRepository = injectionSettingsRepository;
            _logger = logger;
            _processWatcher = processWatcher;
        }

        public async Task UnwatchAsync(AggregateId id)
        {
            using (await _queue.GetBlockingScope())
            {
                var watchableProcess = FindById(id);
                if (watchableProcess != null)
                {
                    _watchableProcesses[watchableProcess].Dispose();
                    _watchableProcesses.Remove(watchableProcess);
                }
            }
        }

        public async Task WatchAsync(AggregateId id)
        {
            using (await _queue.GetBlockingScope())
            {
                var watchableProcess = FindById(id);
                if (watchableProcess == null)
                {
                    watchableProcess = new InjectionSettingsWatchableProcess(
                        id,
                        _plugins,
                        _communicationAddress,
                        _communicationPort,
                        _assembler,
                        _directoryRepository,
                        _injectionService,
                        _injectionSettingRepository,
                        _logger);

                    _watchableProcesses[watchableProcess] = _processWatcher.Watch(watchableProcess);
                }

                await watchableProcess.InitializeAsync();
            }
        }

        private InjectionSettingsWatchableProcess FindById(AggregateId id)
        {
            return (from keyValuePair in _watchableProcesses
                    where keyValuePair.Key.Id == id
                    select keyValuePair.Key).FirstOrDefault();
        }
    }
}