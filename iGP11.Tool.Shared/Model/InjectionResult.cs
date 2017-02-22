using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Model
{
    [DataContract]
    public class InjectionResult
    {
        public InjectionResult(ulong status, string message = null)
        {
            Status = status;
            Message = message;
        }

        [DataMember(Name = "message")]
        public string Message { get; private set; }

        [DataMember(Name = "status")]
        public ulong Status { get; private set; }
    }
}