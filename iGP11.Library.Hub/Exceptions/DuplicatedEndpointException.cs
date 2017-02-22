using System;
using System.Runtime.Serialization;

namespace iGP11.Library.Hub.Exceptions
{
    [Serializable]
    public class DuplicatedEndpointException : Exception
    {
        public DuplicatedEndpointException()
        {
        }

        public DuplicatedEndpointException(string message)
            : base(message)
        {
        }

        public DuplicatedEndpointException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DuplicatedEndpointException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}