using System.Threading.Tasks;

namespace iGP11.Library.Hub.Client.Action
{
    public class LambdaTimeoutHandler : ITimeoutHandler
    {
        private readonly System.Action _action;

        public LambdaTimeoutHandler(System.Action action)
        {
            _action = action;
        }

        public async Task HandleAsync()
        {
            _action?.Invoke();
            await Task.Yield();
        }
    }
}