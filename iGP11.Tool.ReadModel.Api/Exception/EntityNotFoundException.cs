using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.ReadModel.Api.Exception
{
    [Serializable]
    public class EntityNotFoundException : System.Exception
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        public EntityNotFoundException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}