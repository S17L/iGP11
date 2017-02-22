using System;
using System.Threading.Tasks;

using iGP11.Tool.Application.Model;

namespace iGP11.Tool.Infrastructure.Communication
{
    internal interface ICommunicationProxy : IDisposable
    {
        Task<CommunicationResult<string>> PublishAsync(string request);
    }
}