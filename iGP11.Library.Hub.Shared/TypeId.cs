using System;
using System.Runtime.Serialization;

namespace iGP11.Library.Hub.Shared
{
    [DataContract]
    [Serializable]
    public struct TypeId : IEquatable<TypeId>
    {
        private TypeId(string id)
        {
            Id = id;
        }

        [DataMember(Name = "id", IsRequired = true)]
        public string Id { get; private set; }

        public static TypeId Generate<TEvent>()
        {
            return Generate(typeof(TEvent));
        }

        public static TypeId Generate(Type type)
        {
            return new TypeId(type.AssemblyQualifiedName);
        }

        public static TypeId Generate(string id)
        {
            return new TypeId(id);
        }

        public static bool operator ==(TypeId x, TypeId y)
        {
            return x.Id == y.Id;
        }

        public static bool operator !=(TypeId x, TypeId y)
        {
            return !(x == y);
        }

        public bool Equals(TypeId other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object other)
        {
            return other is TypeId && Equals((TypeId)other);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return Id;
        }
    }
}