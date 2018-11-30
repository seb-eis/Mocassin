using System;
using System.Runtime.Serialization;

namespace Mocassin.Model.Mml.Exceptions
{
    /// <summary>
    ///     Exception that is thrown if the target of an export attribute usage does not match the requirements for a
    ///     successful export or the export attribute usage is invalid
    /// </summary>
    public class InvalidExportException : Exception
    {
        /// <inheritdoc />
        public InvalidExportException()
        {
        }

        /// <inheritdoc />
        public InvalidExportException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        public InvalidExportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc />
        protected InvalidExportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}