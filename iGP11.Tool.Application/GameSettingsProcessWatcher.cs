using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.Component;
using iGP11.Library.DDD;
using iGP11.Tool.Application.Bootstrapper;
using iGP11.Tool.Domain.Model.Directory;
using iGP11.Tool.Domain.Model.GameSettings;

namespace iGP11.Tool.Application
{
    public class GameSettingsProcessWatcher
    {
        private readonly ComponentAssembler _assembler;
        private readonly string _communicationAddress;
        private readonly ushort _communicationPort;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IInjectionService _injectionService;
        private readonly ILogger _logger;
        private readonly Plugins _plugins;
        private readonly IProcessWatcher _processWatcher;
        private readonly BlockingTaskQueue _queue = new BlockingTaskQueue();
        private readonly IDictionary<GameSettingsWatchableProcess, IDisposable> _watchableProcesses = new Dictionary<GameSettingsWatchableProcess, IDisposable>();

        public GameSettingsProcessWatcher(
            Plugins plugins,
            string communicationAddress,
            ushort communicationPort,
            ComponentAssembler assembler,
            IDirectoryRepository directoryRepository,
            IGameRepository gameRepository,
            IInjectionService injectionService,
            ILogger logger,
            IProcessWatcher processWatcher)
        {
            _plugins = plugins;
            _communicationAddress = communicationAddress;
            _communicationPort = communicationPort;
            _assembler = assembler;
            _directoryRepository = directoryRepository;
            _injectionService = injectionService;
            _gameRepository = gameRepository;
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
                    watchableProcess = new GameSettingsWatchableProcess(
                        id,
                        _plugins,
                        _communicationAddress,
                        _communicationPort,
                        _assembler,
                        _directoryRepository,
                        _gameRepository,
                        _injectionService,
                        _logger);

                    _watchableProcesses[watchableProcess] = _processWatcher.Watch(watchableProcess);
                }

                await watchableProcess.InitializeAsync();
            }
        }

        private GameSettingsWatchableProcess FindById(AggregateId id)
        {
            return (from keyValuePair in _watchableProcesses
                    where keyValuePair.Key.GameId == id
                    select keyValuePair.Key).FirstOrDefault();
        }
    }
}