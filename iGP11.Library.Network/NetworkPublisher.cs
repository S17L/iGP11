using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace iGP11.Library.Network
{
    public class NetworkPublisher : IPublisher
    {
        private readonly string _serverUri;
        private readonly int _timeout;

        public NetworkPublisher(string serverUri, int timeout = 5000)
        {
            _serverUri = serverUri;
            _timeout = timeout;
        }

        public async Task<CommandOutput> PublishAsync(Command command)
        {
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                client.Timeout = TimeSpan.FromMilliseconds(_timeout);
                var response = await client.PostAsync(_serverUri, new StringContent(command.Serialize()));
                var data = await response.Content.ReadAsStringAsync();

                return new CommandOutput(data);
            }
        }
    }
}