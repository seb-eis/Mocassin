using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ICon.Framework.Exceptions
{
    /// <inheritdoc />
    /// <remarks> Abstract base class for all custom errors. Do not use to encapsulate basic .NET exceptions </remarks>
    public abstract class CustomException : Exception
    {
        /// <inheritdoc />
        protected CustomException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        protected CustomException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc />
        protected CustomException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public abstract override string ToString();
    }
}
