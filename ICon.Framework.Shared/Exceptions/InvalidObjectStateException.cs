using System;
using System.Runtime.Serialization;

namespace Mocassin.Framework.Exceptions
{
    /// <summary>
    ///     Exception thrown if a method/property call fails due to its internal state rather than the passed parameters (e.g.
    ///     unset fields or flags)
    /// </summary>
    public class InvalidObjectStateException : CustomException
    {
        /// <summary>
        ///     The name of the invalid property or field
        /// </summary>
        public string DataMemberName { get; }

        /// <summary>
        ///     Creates new exception from the specified message
        /// </summary>
        /// <param name="message"></param>
        public InvalidObjectStateException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Creates new exception from the specified message and the data member name that was invalid
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dataMemberName"></param>
        public InvalidObjectStateException(string message, string dataMemberName)
            : base(message)
        {
            DataMemberName = dataMemberName;
        }

        /// <summary>
        ///     Creates new exception from the specified message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public InvalidObjectStateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Creates new exception from serialization info and a streaming context
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InvalidObjectStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"An operation failed due to an unexpected or invalid object state{Environment.NewLine}Details:{Environment.NewLine}{Message}";
        }
    }
}