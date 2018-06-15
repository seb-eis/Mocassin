using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ICon.Framework.Exceptions
{
    /// <summary>
    /// Exception thrown if a method/property access to an object tries to change an inherited field/property that is immutable in the implementing type
    /// </summary>
    public class InvalidObjectChangeException : IConCustomException
    {
        /// <summary>
        /// The name of the immutable property or field
        /// </summary>
        public String DataMemberName { get; }

        /// <summary>
        /// Creates new exception from the specified message
        /// </summary>
        /// <param name="message"></param>
        public InvalidObjectChangeException(String message) : base(message)
        {
        }

        /// <summary>
        /// Creates new exception from the specified message and the data member name that was invalid
        /// </summary>
        /// <param name="message"></param>
        public InvalidObjectChangeException(String message, String dataMemberName) : base(message)
        {
            DataMemberName = dataMemberName;
        }

        /// <summary>
        /// Creates new exception from the specified message and inner exception
        /// </summary>
        /// <param name="message"></param>
        public InvalidObjectChangeException(String message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates new exception from serialization info and a streaming context
        /// </summary>
        /// <param name="message"></param>
        protected InvalidObjectChangeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Overrides object ToString() method
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return $"An invalid object state change attempt occured:{Environment.NewLine}Details{Environment.NewLine}{Message}";
        }
    }
}
