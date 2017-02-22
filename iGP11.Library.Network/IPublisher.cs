using System.Threading.Tasks;

namespace iGP11.Library.Network
{
    public interface IPublisher
    {
        Task<CommandOutput> PublishAsync(Command command);
    }
}