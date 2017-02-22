using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Notification;

using ApplicationModel = iGP11.Tool.Application.Model;
using ProxySettingsLoadedEvent = iGP11.Tool.Shared.Event.ProxySettingsLoadedEvent;

namespace iGP11.Tool.Application.CommandHandler
{
    public class UpdateProxySettingsCommandHandler : IDomainCommandHandler<UpdateProxySettingsCommand>
    {
        private readonly ICommunicatorFactory _communicatorFactory;

        public UpdateProxySettingsCommandHandler(ICommunicatorFactory communicatorFactory)
        {
            _communicatorFactory = communicatorFactory;
        }

        public async Task HandleAsync(DomainCommandContext context, UpdateProxySettingsCommand command)
        {
            using (var communicator = _communicatorFactory.Create())
            {
                var result = await communicator.UpdateAsync(command.ProxyPluginSettings.Map<ApplicationModel.ProxyPluginSettings>());
                if (result)
                {
                    var communicationResult = await communicator.GetProxySettingsAsync();
                    if (communicationResult.IsCompleted)
                    {
                        await context.PublishAsync(new ProxySettingsLoadedEvent(communicationResult.Response.Map<ProxySettings>()));
                        return;
                    }
                }

                await context.EmitAsync(new ErrorOccuredEvent());
            }
        }
    }
}