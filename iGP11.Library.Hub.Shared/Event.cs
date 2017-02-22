using System.Runtime.Serialization;

namespace iGP11.Library.Hub.Shared
{
    [DataContract]
    public class Event
    {
        public Event(TypeId typeId, string data)
        {
            TypeId = typeId;
            Data = data;
        }

        [DataMember(Name = "data", IsRequired = true)]
        public string Data { get; set; }

        [DataMember(Name = "typeId", IsRequired = true)]
        public TypeId TypeId { get; set; }

        public override string ToString()
        {
            return TypeId.Id;
        }
    }
}