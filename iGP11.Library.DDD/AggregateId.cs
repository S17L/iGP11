using System;
using System.Runtime.Serialization;

namespace iGP11.Library.DDD
{
    [DataContract]
    public class AggregateId
    {
        public AggregateId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("Value cannot be empty", nameof(value));
            }

            Value = value;
        }

        [DataMember(Name = "value")]
        public Guid Value { get; private set; }

        public static AggregateId Generate()
        {
            return new AggregateId(Guid.NewGuid());
        }

        public static bool operator ==(AggregateId x, AggregateId y)
        {
            return (ReferenceEquals(x, null) && ReferenceEquals(y, null)) || (!ReferenceEquals(x, null) && !ReferenceEquals(y, null) && (x.Value == y.Value));
        }

        public static implicit operator Guid(AggregateId aggregateId)
        {
            return aggregateId.Value;
        }

        public static implicit operator AggregateId(Guid id)
        {
            return new AggregateId(id);
        }

        public static bool operator !=(AggregateId x, AggregateId y)
        {
            return !(x == y);
        }

        public override bool Equals(object @object)
        {
            var aggregateId = @object as AggregateId;

            return (aggregateId != null) && (Value == aggregateId.Value);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString("B");
        }
    }
}