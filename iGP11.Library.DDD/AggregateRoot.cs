using System.Runtime.Serialization;

namespace iGP11.Library.DDD
{
    [DataContract]
    public abstract class AggregateRoot<TAggregateId>
    {
        protected AggregateRoot(TAggregateId id)
        {
            Id = id;
        }

        [DataMember(Name = "id")]
        public TAggregateId Id { get; private set; }
    }
}