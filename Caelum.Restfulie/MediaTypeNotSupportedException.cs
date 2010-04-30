using System;
using System.Runtime.Serialization;

namespace Caelum.Restfulie
{
    public class MediaTypeNotSupportedException : Exception
    {
        public MediaTypeNotSupportedException()
        {
        }

        public MediaTypeNotSupportedException(string message)
            : base(message)
        {
        }

        public MediaTypeNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MediaTypeNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}