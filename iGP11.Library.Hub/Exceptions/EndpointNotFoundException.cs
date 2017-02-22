using System;
using System.Runtime.Serialization;

namespace iGP11.Library.Hub.Exceptions
{
    [Serializable]
    public class EndpointNotFoundException : Exception
    {
        public EndpointNotFoundException()
        {
        }

        public EndpointNotFoundException(string message)
            : base(message)
        {
        }

        public EndpointNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected EndpointNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}