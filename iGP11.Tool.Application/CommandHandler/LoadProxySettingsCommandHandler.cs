using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Notification;

using ProxySettingsLoadedEvent = iGP11.Tool.Shared.Event.ProxySettingsLoadedEvent;

namespace iGP11.Tool.Application.CommandHandler
{
    public class LoadProxySettingsCommandHandler : IDomainCommandHandler<LoadProxySettingsCommand>
    {
        private readonly ICommunicatorFactory _communicatorFactory;

        public LoadProxySettingsCommandHandler(ICommunicatorFactory communicatorFactory)
        {
            _communicatorFactory = communicatorFactory;
        }

        public async Task HandleAsync(DomainCommandContext context, LoadProxySettingsCommand command)
        {
            using (var communicator = _communicatorFactory.Create())
            {
                var result = await communicator.GetProxySettingsAsync();
                if (result.IsCompleted)
                {
                    await context.PublishAsync(new ProxySettingsLoadedEvent(result.Response.Map<ProxySettings>()));
                    return;
                }

                await context.EmitAsync(new ErrorOccuredNotification());
            }
        }
    }
}