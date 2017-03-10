using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Domain.Exceptions
{
    [Serializable]
    public class GameTemplateNotFoundException : DomainOperationException
    {
        public GameTemplateNotFoundException()
        {
        }

        public GameTemplateNotFoundException(string message)
            : base(message)
        {
        }

        public GameTemplateNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected GameTemplateNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}