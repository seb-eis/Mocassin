using System;
using System.Runtime.Serialization;

namespace Mocassin.Model.Mml.Exceptions
{
    /// <summary>
    ///     Exception that is thrown if the target of an import attribute usage does not match the requirements for a
    ///     successful import or the import attribute usage is invalid
    /// </summary>
    public class InvalidImportException : Exception
    {
        /// <inheritdoc />
        public InvalidImportException()
        {
        }

        /// <inheritdoc />
        public InvalidImportException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        public InvalidImportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc />
        protected InvalidImportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}