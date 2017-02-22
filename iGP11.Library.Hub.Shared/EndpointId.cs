using System;
using System.Runtime.Serialization;

namespace iGP11.Library.Hub.Shared
{
    [DataContract]
    [Serializable]
    public struct EndpointId : IEquatable<EndpointId>
    {
        private EndpointId(Guid id)
        {
            Id = id;
        }

        [DataMember(Name = "id", IsRequired = true)]
        public Guid Id { get; private set; }

        public static EndpointId Generate()
        {
            return new EndpointId(Guid.NewGuid());
        }

        public static bool operator ==(EndpointId x, EndpointId y)
        {
            return x.Id == y.Id;
        }

        public static bool operator !=(EndpointId x, EndpointId y)
        {
            return !(x == y);
        }

        public bool Equals(EndpointId other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object other)
        {
            return other is EndpointId && Equals((EndpointId)other);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return Id.ToString("B");
        }
    }
}