using System.Threading.Tasks;

using iGP11.Library.DDD.Action;

namespace iGP11.Library.DDD
{
    public interface IDomainCommandHandler<in TCommand>
    {
        Task HandleAsync(DomainCommandContext context, TCommand command);
    }
}