using System;
using System.Runtime.Serialization;

namespace Timelinelite.Core
{
    public abstract class GCUException : Exception
    {
        protected GCUException()
        {
        }

        protected GCUException(string message) : base(message)
        {
        }

        protected GCUException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GCUException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}