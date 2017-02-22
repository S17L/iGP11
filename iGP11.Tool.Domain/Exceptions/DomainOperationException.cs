using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Domain.Exceptions
{
    [Serializable]
    public class DomainOperationException : Exception
    {
        public DomainOperationException()
        {
        }

        public DomainOperationException(string message)
            : base(message)
        {
        }

        public DomainOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DomainOperationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}