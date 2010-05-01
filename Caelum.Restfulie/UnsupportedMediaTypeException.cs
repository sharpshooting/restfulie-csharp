using System;
using System.Runtime.Serialization;

namespace Caelum.Restfulie
{
    public class UnsupportedMediaTypeException : Exception
    {
        public UnsupportedMediaTypeException()
        {
        }

        public UnsupportedMediaTypeException(string message)
            : base(message)
        {
        }

        public UnsupportedMediaTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UnsupportedMediaTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}