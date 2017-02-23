using iGP11.Library;
using iGP11.Tool.Application;

namespace iGP11.Tool.Infrastructure.Communication
{
    public class CommunicatorFactory : ICommunicatorFactory
    {
        private readonly string _address;
        private readonly ushort _port;
        private readonly ILogger _logger;

        public CommunicatorFactory(string address, ushort port, ILogger logger)
        {
            _address = address;
            _port = port;
            _logger = logger;
        }

        public ICommunicator Create()
        {
            return new Communicator(new CommunicationProxyFactory(_address, _port, _logger));
        }
    }
}