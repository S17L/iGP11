using System;
using System.Runtime.Serialization;

namespace iGP11.Library.Hub.Shared
{
    [DataContract]
    [Serializable]
    public struct EventId : IEquatable<EventId>
    {
        private EventId(Guid id)
        {
            Id = id;
        }

        [DataMember(Name = "id", IsRequired = true)]
        public Guid Id { get; private set; }

        public static EventId Generate()
        {
            return new EventId(Guid.NewGuid());
        }

        public static bool operator ==(EventId x, EventId y)
        {
            return x.Id == y.Id;
        }

        public static bool operator !=(EventId x, EventId y)
        {
            return !(x == y);
        }

        public bool Equals(EventId other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object other)
        {
            return other is EventId && Equals((EventId)other);
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