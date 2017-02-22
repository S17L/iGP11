using System.Runtime.Serialization;

namespace iGP11.Library.Network
{
    [DataContract]
    public class Command
    {
        public Command(string name, string data)
        {
            Name = name;
            Data = data;
        }

        [DataMember(Name = "data", IsRequired = true)]
        public string Data { get; private set; }

        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; private set; }
    }
}