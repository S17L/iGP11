using System.Threading.Tasks;

using iGP11.Library.Hub.Client.Action;

namespace iGP11.Library.DDD.Action
{
    public class DomainEventContext
    {
        private readonly FeedbackEventContext _context;

        public DomainEventContext(FeedbackEventContext context)
        {
            _context = context;
        }

        public async Task EmitAsync<TEvent>(TEvent @event)
        {
            await _context.EmitAsync(@event);
        }
    }
}