using System;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client
{
    public interface IListener : IDisposable
    {
        TypeId TypeId { get; }
    }
}