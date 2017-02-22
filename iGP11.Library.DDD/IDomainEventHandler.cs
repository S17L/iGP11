using System.Threading.Tasks;

using iGP11.Library.DDD.Action;

namespace iGP11.Library.DDD
{
    public interface IDomainEventHandler<in TEvent>
    {
        Task HandleAsync(DomainEventContext context, TEvent @event);
    }
}