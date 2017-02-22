using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Domain.Exceptions
{
    [Serializable]
    public class ProfileTemplateNotFoundException : DomainOperationException
    {
        public ProfileTemplateNotFoundException()
        {
        }

        public ProfileTemplateNotFoundException(string message)
            : base(message)
        {
        }

        public ProfileTemplateNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ProfileTemplateNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}