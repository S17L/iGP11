using iGP11.Tool.Application;

namespace iGP11.Tool.Infrastructure.Communication
{
    public class CommunicatorFactory : ICommunicatorFactory
    {
        private readonly string _address;
        private readonly ushort _port;

        public CommunicatorFactory(string address, ushort port)
        {
            _address = address;
            _port = port;
        }

        public ICommunicator Create()
        {
            return new Communicator(new CommunicationProxyFactory(_address, _port));
        }
    }
}