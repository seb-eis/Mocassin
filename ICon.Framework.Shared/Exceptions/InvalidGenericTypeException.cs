using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ICon.Framework.Exceptions
{
    /// <summary>
    /// Exception thrown if generic components or methods do not support the specified type
    /// </summary>
    public class InvalidGenericTypeException : IConCustomException
    {
        /// <summary>
        /// The type of the parameter
        /// </summary>
        public Type ParameterType { get; }

        /// <summary>
        /// Creates new exception from the specified message
        /// </summary>
        /// <param name="message"></param>
        public InvalidGenericTypeException(String message) : base(message)
        {
        }

        /// <summary>
        /// Creates new exception from the specified message and the data member name that was invalid
        /// </summary>
        /// <param name="message"></param>
        public InvalidGenericTypeException(String message, Type parameterType) : base(message)
        {
            ParameterType = parameterType;
        }

        /// <summary>
        /// Creates new exception from the specified message and inner exception
        /// </summary>
        /// <param name="message"></param>
        public InvalidGenericTypeException(String message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates new exception from serialization info and a streaming context
        /// </summary>
        /// <param name="message"></param>
        protected InvalidGenericTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Overrides object ToString() method
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return $"An invalid generic usage occured:{Environment.NewLine}Details{Environment.NewLine}{Message}";
        }
    }
}
