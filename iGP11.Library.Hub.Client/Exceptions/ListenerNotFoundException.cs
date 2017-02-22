using System;
using System.Runtime.Serialization;

namespace iGP11.Library.Hub.Client.Exceptions
{
    [Serializable]
    public class ListenerNotFoundException : Exception
    {
        public ListenerNotFoundException()
        {
        }

        public ListenerNotFoundException(string message)
            : base(message)
        {
        }

        public ListenerNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ListenerNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}