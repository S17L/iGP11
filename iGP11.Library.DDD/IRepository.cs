using System.Threading.Tasks;

namespace iGP11.Library.DDD
{
    public interface IRepository<TAggregateRoot, in TAggregateId>
    {
        Task<TAggregateRoot> LoadAsync(TAggregateId id);

        Task SaveAsync(TAggregateRoot aggregateRoot);
    }
}