using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Exceptions;

namespace ICon.Framework.Exceptions
{
    /// <summary>
    /// Exception thrown if a generic manager input function is called with an unexpected type the manager is not designed to handle
    /// </summary>
    public class UnexpectedInputTypeException : IConCustomException
    {
        /// <summary>
        /// The actual model object type that caused the exception
        /// </summary>
        public Type UnexpectedType { get; }

        /// <summary>
        /// Creates new unexpected type exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="unexpectedType"></param>
        public UnexpectedInputTypeException(String message, Type unexpectedType) : base(message)
        {
            UnexpectedType = unexpectedType;
        }

        /// <summary>
        /// Creates info string from exception, overrides obejcts ToString()
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return $"Model input manager was provided with an unexcpected type{Environment.NewLine} Type: {UnexpectedType.ToString()}{Environment.NewLine} Details:{Environment.NewLine}{Message}";
        }
    }
}
