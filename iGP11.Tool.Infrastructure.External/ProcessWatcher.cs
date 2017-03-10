using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

using iGP11.Library;
using iGP11.Tool.Application;

namespace iGP11.Tool.Infrastructure.External
{
    public sealed class ProcessWatcher : IProcessWatcher
    {
        private const int MaxPathLength = 260;
        private const string Query = "SELECT * FROM __InstanceOperationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'";
        private const string Scope = @"\\.\root\CIMV2";

        private readonly object _lock = new object();
        private readonly ILogger _logger;
        private readonly ICollection<IWatchableProcess> _watchableProcesses = new List<IWatchableProcess>();
        private readonly ManagementEventWatcher _watcher;

        public ProcessWatcher(ILogger logger)
        {
            _logger = logger;
            _watcher = new ManagementEventWatcher(Scope, Query);
            _watcher.EventArrived += OnEventArrived;
            _watcher.Start();
        }

        public void Dispose()
        {
            _watcher.Stop();
            _watcher.Dispose();
        }

        public IDisposable Watch(IWatchableProcess watchableProcess)
        {
            lock (_lock)
            {
                _watchableProcesses.Add(watchableProcess);
                return new UnwatchWatcher(this, watchableProcess);
            }
        }

        private IWatchableProcess GetWatchableProcess(EventArrivedEventArgs eventArgs)
        {
            var @object = (ManagementBaseObject)eventArgs.NewEvent.Properties["TargetInstance"].Value;
            var pid = (uint)@object.Properties["ProcessID"].Value;

            var builder = new StringBuilder(MaxPathLength + 1);
            var filePath = InjectionDriver.GetProcessFilePath(pid, builder, builder.Capacity) ? builder.ToString() : null;

            _logger.Log(LogLevel.Debug, $"wmi event received, file path: {filePath}, pid: {pid}");

            lock (_lock)
            {
                return _watchableProcesses.FirstOrDefault(process => string.Equals(process.FilePath, filePath, StringComparison.OrdinalIgnoreCase));
            }
        }

        private void OnEventArrived(object sender, EventArrivedEventArgs e)
        {
            try
            {
                var eventName = e.NewEvent.ClassPath.ClassName;
                if (string.Compare(eventName, "__InstanceCreationEvent", StringComparison.Ordinal) == 0)
                {
                    var watchableProcess = GetWatchableProcess(e);
                    if (watchableProcess == null)
                    {
                        return;
                    }

                    _logger.Log(LogLevel.Information, $"process: {watchableProcess.FilePath} started; executing process watcher handler...");
                    watchableProcess.OnStarted();
                }
                else if (string.Compare(eventName, "__InstanceDeletionEvent", StringComparison.Ordinal) == 0)
                {
                    var watchableProcess = GetWatchableProcess(e);
                    if (watchableProcess == null)
                    {
                        return;
                    }

                    _logger.Log(LogLevel.Information, $"process: {watchableProcess.FilePath} terminated; executing process watcher handler...");
                    watchableProcess.OnTerminated();
                }
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, $"exception occured while processing wmi event; exception: {exception}");
            }
        }

        private sealed class UnwatchWatcher : IDisposable
        {
            private readonly IWatchableProcess _watchableProcess;
            private readonly ProcessWatcher _watcher;

            public UnwatchWatcher(ProcessWatcher watcher, IWatchableProcess watchableProcess)
            {
                _watcher = watcher;
                _watchableProcess = watchableProcess;
            }

            public void Dispose()
            {
                lock (_watcher._lock)
                {
                    _watcher._watchableProcesses.Remove(_watchableProcess);
                }
            }
        }
    }
}