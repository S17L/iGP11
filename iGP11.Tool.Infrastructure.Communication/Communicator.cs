using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Tool.Application;
using iGP11.Tool.Infrastructure.Communication.Enums;
using iGP11.Tool.Infrastructure.Communication.Model;
using iGP11.Tool.Shared.Model;

using ApplicationModel = iGP11.Tool.Application.Model;

namespace iGP11.Tool.Infrastructure.Communication
{
    internal sealed class Communicator : ICommunicator
    {
        private readonly ICommunicationProxy _communicationProxy;

        private bool _isDisposed;

        public Communicator(ICommunicationProxyFactory communicationProxyFactory)
        {
            _communicationProxy = communicationProxyFactory.Create();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            try
            {
                _communicationProxy.Dispose();
            }
            finally
            {
                _isDisposed = true;
            }
        }

        public async Task<ApplicationModel.CommunicationResult<ActivationStatus>> GetActivationStatusAsync(PluginType pluginType)
        {
            var result = await Publish(
                             RequestType.GetActivationStatus,
                             pluginType.Serialize());

            var status = result.IsCompleted
                             ? (ActivationStatus)int.Parse(result.Response)
                             : ActivationStatus.NotRetrievable;

            return new ApplicationModel.CommunicationResult<ActivationStatus>(result.IsCompleted, status);
        }

        public async Task<ApplicationModel.CommunicationResult<ApplicationModel.ProxySettings>> GetProxySettingsAsync()
        {
            var result = await Publish(
                             RequestType.GetProxySettings,
                             string.Empty);

            var state = result.IsCompleted
                            ? result.Response.Deserialize<ApplicationModel.ProxySettings>()
                            : null;

            return new ApplicationModel.CommunicationResult<ApplicationModel.ProxySettings>(result.IsCompleted, state);
        }

        public async Task<bool> UpdateAsync(ApplicationModel.ProxyPluginSettings settings)
        {
            var result = await Publish(
                             RequestType.UpdateProxySettings,
                             settings.Serialize());

            return result.IsCompleted && (int.Parse(result.Response) > 0);
        }

        private async Task<ApplicationModel.CommunicationResult<string>> Publish(RequestType requestType, string data)
        {
            var command = new Command
            {
                Id = (int)requestType,
                Data = data
            };

            return await _communicationProxy.PublishAsync(command.Serialize());
        }
    }
}