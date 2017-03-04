using System;

namespace iGP11.Tool.Application
{
    public interface IProcessWatcher : IDisposable
    {
        IDisposable Watch(IWatchableProcess watchableProcess);
    }
}