using System;
using System.Threading.Tasks;

namespace iGP11.Library.Hub.Client.Action
{
    public class ThrowExceptionTimeoutHandler : ITimeoutHandler
    {
        public Task HandleAsync()
        {
            throw new TimeoutException();
        }
    }
}