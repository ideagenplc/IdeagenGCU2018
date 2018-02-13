using System;
using System.Runtime.Serialization;

namespace Timelinelite.Core
{
    public class AuthenticationException : GCUException
    {
        public AuthenticationException()
        {
        }

        public AuthenticationException(string message) : base(message)
        {
        }

        public AuthenticationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public AuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}