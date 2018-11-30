using System;
using System.Runtime.Serialization;

namespace Mocassin.Model.Mml.Exceptions
{
    /// <summary>
    ///     Exception that is thrown if attribute usage is ill or ambiguously defined or an attribute is used in an illegal way
    /// </summary>
    public class InvalidAttributeException : Exception
    {
        /// <inheritdoc />
        public InvalidAttributeException()
        {
        }

        /// <inheritdoc />
        public InvalidAttributeException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        public InvalidAttributeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc />
        protected InvalidAttributeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}