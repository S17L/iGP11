using System.Threading.Tasks;

namespace iGP11.Library.EventPublisher
{
    public interface IEventHandler<in TEvent>
    {
        Task HandleAsync(TEvent @event);
    }
}