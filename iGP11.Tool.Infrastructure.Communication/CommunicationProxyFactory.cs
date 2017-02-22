namespace iGP11.Tool.Infrastructure.Communication
{
    internal class CommunicationProxyFactory : ICommunicationProxyFactory
    {
        private readonly string _address;
        private readonly ushort _port;

        public CommunicationProxyFactory(string address, ushort port)
        {
            _address = address;
            _port = port;
        }

        public ICommunicationProxy Create()
        {
            return new CommunicationProxy(_address, _port);
        }
    }
}