using System;
using System.Runtime.Serialization;

namespace Mocassin.Framework.Exceptions
{
    /// <inheritdoc />
    /// <remarks> Abstract base class for all custom errors. Do not use to encapsulate basic .NET exceptions </remarks>
    public abstract class MocassinException : Exception
    {
        /// <inheritdoc />
        protected MocassinException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        protected MocassinException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc />
        protected MocassinException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc />
        public abstract override string ToString();
    }
}