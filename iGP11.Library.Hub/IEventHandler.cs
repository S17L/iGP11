using System.Threading.Tasks;

namespace iGP11.Library.Hub
{
    public interface IEventHandler<in TEvent>
    {
        Task HandleAsync(TEvent @event);
    }
}