using System;
using System.Runtime.Serialization;

namespace iGP11.Library.Hub.Client.Exceptions
{
    [Serializable]
    public class HubNotFoundException : Exception
    {
        public HubNotFoundException()
        {
        }

        public HubNotFoundException(string message)
            : base(message)
        {
        }

        public HubNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected HubNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}