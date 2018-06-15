using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ICon.Framework.Exceptions
{
    /// <summary>
    /// Exception thrown if a method/property call fails due to its internal state rather than the passed parameters (e.g. unset fields or flags)
    /// </summary>
    public class InvalidObjectStateException : IConCustomException
    {
        /// <summary>
        /// The name of the invalid property or field
        /// </summary>
        public String DataMemberName { get; } 

        /// <summary>
        /// Creates new exception from the specified message
        /// </summary>
        /// <param name="message"></param>
        public InvalidObjectStateException(String message) : base(message)
        {
        }

        /// <summary>
        /// Creates new exception from the specified message and the data member name that was invalid
        /// </summary>
        /// <param name="message"></param>
        public InvalidObjectStateException(String message, String dataMemberName) : base(message)
        {
            DataMemberName = dataMemberName;
        }

        /// <summary>
        /// Creates new exception from the specified message and inner exception
        /// </summary>
        /// <param name="message"></param>
        public InvalidObjectStateException(String message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates new exception from serialization info and a streaming context
        /// </summary>
        /// <param name="message"></param>
        protected InvalidObjectStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Overrides object ToString() method
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return $"An operation failed due to an unexpected or invalid object state{Environment.NewLine}Details:{Environment.NewLine}{Message}";
        }
    }
}
