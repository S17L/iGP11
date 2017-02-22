using System.Threading.Tasks;

namespace iGP11.Library.EventPublisher
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event);

        void Register<TEvent>(IEventHandler<TEvent> eventHandler);

        void Unregister<TEvent>(IEventHandler<TEvent> eventHandler);
    }
}