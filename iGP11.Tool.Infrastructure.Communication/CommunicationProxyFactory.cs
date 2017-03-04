using iGP11.Library;

namespace iGP11.Tool.Infrastructure.Communication
{
    internal class CommunicationProxyFactory : ICommunicationProxyFactory
    {
        private readonly string _address;
        private readonly ILogger _logger;
        private readonly ushort _port;

        public CommunicationProxyFactory(string address, ushort port, ILogger logger)
        {
            _address = address;
            _port = port;
            _logger = logger;
        }

        public ICommunicationProxy Create()
        {
            return new CommunicationProxy(_address, _port, _logger);
        }
    }
}