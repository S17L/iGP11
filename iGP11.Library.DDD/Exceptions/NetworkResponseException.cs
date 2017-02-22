using System;
using System.Runtime.Serialization;

namespace iGP11.Library.DDD.Exceptions
{
    [Serializable]
    public class NetworkResponseException : Exception
    {
        public NetworkResponseException()
        {
        }

        public NetworkResponseException(string message)
            : base(message)
        {
        }

        public NetworkResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NetworkResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}