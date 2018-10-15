using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ICon.Framework.Exceptions
{
    /// <summary>
    /// Exception thrown if a method/property access to an object tries to change an inherited field/property that is immutable in the implementing type
    /// </summary>
    public class InvalidStateChangeException : CustomException
    {
        /// <summary>
        /// The name of the immutable property or field
        /// </summary>
        public string DataMemberName { get; }

        /// <inheritdoc />
        public InvalidStateChangeException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public InvalidStateChangeException(string message, string dataMemberName) : base(message)
        {
            DataMemberName = dataMemberName;
        }

        /// <inheritdoc />
        public InvalidStateChangeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc />
        protected InvalidStateChangeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"An invalid object state change attempt occured:{Environment.NewLine}Details{Environment.NewLine}{Message}";
        }
    }
}
