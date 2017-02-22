using System;
using System.Threading.Tasks;

namespace iGP11.Library.Hub.Client.Action
{
    public class AsynchronousLambdaTimeoutHandler : ITimeoutHandler
    {
        private readonly Func<Task> _action;

        public AsynchronousLambdaTimeoutHandler(Func<Task> action)
        {
            _action = action;
        }

        public async Task HandleAsync()
        {
            if (_action != null)
            {
                await _action();
            }
        }
    }
}