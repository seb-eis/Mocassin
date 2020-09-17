using System;
using System.Runtime.Serialization;

namespace Mocassin.Framework.Exceptions
{
    /// <summary>
    ///     Exception thrown if generic components or methods do not support the specified type
    /// </summary>
    public class InvalidGenericTypeException : MocassinException
    {
        /// <summary>
        ///     The type of the parameter
        /// </summary>
        public Type ParameterType { get; }

        /// <inheritdoc />
        public InvalidGenericTypeException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        public InvalidGenericTypeException(string message, Type parameterType)
            : base(message)
        {
            ParameterType = parameterType;
        }

        /// <inheritdoc />
        public InvalidGenericTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc />
        protected InvalidGenericTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc />
        public override string ToString() => $"An invalid generic usage occured:{Environment.NewLine}Details{Environment.NewLine}{Message}";
    }
}