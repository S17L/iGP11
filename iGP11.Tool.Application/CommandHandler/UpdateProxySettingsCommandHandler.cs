using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Notification;

using ApplicationModel = iGP11.Tool.Application.Model;
using SharedEvent = iGP11.Tool.Shared.Event;

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
            if (await UpdateAsync(command.ProxyPluginSettings))
            {
                var settings = await GetProxySettingsAsync();
                if (settings != null)
                {
                    await context.PublishAsync(new SharedEvent.ProxySettingsLoadedEvent(settings));
                    return;
                }
            }

            await context.EmitAsync(new ErrorOccuredNotification());
        }

        private async Task<ProxySettings> GetProxySettingsAsync()
        {
            using (var communicator = _communicatorFactory.Create())
            {
                var communicationResult = await communicator.GetProxySettingsAsync();
                return communicationResult.IsCompleted
                           ? communicationResult.Response.Map<ProxySettings>()
                           : null;
            }
        }

        private async Task<bool> UpdateAsync(ProxyPluginSettings settings)
        {
            using (var communicator = _communicatorFactory.Create())
            {
                return await communicator.UpdateAsync(settings.Map<ApplicationModel.ProxyPluginSettings>());
            }
        }
    }
}