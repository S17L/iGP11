using System.Runtime.Serialization;

namespace iGP11.Tool.Infrastructure.Communication.Model
{
    [DataContract]
    internal class Command
    {
        [DataMember(Name = "data")]
        public string Data { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }
    }
}