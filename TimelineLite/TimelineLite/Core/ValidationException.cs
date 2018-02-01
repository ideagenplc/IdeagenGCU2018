using System;
using System.Runtime.Serialization;

namespace TimelineLite.Core
{
    public class ValidationException : GCUException
    {
        public ValidationException()
        {
        }

        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}