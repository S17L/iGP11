namespace iGP11.Library.Network
{
    public interface INetworkCommandHandler
    {
        bool Handle(Command command, ref CommandOutput output);
    }
}