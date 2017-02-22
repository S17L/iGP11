using System;

namespace iGP11.Library.Network
{
    public interface IListener : IDisposable
    {
        void Start();

        void Stop();
    }
}