using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Domain.Exceptions
{
    [Serializable]
    public class AggregateRootNotFoundException : DomainOperationException
    {
        public AggregateRootNotFoundException()
        {
        }

        public AggregateRootNotFoundException(string message)
            : base(message)
        {
        }

        public AggregateRootNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AggregateRootNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}