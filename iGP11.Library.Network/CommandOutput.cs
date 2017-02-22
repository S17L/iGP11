using System.Runtime.Serialization;

namespace iGP11.Library.Network
{
    [DataContract]
    public class CommandOutput
    {
        public CommandOutput(string data)
        {
            Data = data;
        }

        [DataMember(Name = "data", IsRequired = true)]
        public string Data { get; private set; }
    }
}