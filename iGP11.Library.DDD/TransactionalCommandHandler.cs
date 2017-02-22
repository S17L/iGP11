using System.Threading.Tasks;
using System.Transactions;

using iGP11.Library.DDD.Action;

namespace iGP11.Library.DDD
{
    public sealed class TransactionalCommandHandler<TCommand> : IDomainCommandHandler<TCommand>
    {
        private readonly IDomainCommandHandler<TCommand> _domainCommandHandler;

        public TransactionalCommandHandler(IDomainCommandHandler<TCommand> domainCommandHandler)
        {
            _domainCommandHandler = domainCommandHandler;
        }

        public async Task HandleAsync(DomainCommandContext context, TCommand command)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _domainCommandHandler.HandleAsync(context, command);
                transaction.Complete();
            }
        }
    }
}