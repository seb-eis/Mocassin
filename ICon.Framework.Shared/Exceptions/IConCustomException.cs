using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ICon.Framework.Exceptions
{
    /// <summary>
    /// Abstract base class for all custom exceptions thrown within the framework (Do not use to encapsulate typical .NET Framework exceptions e.g. ArgumentException)
    /// </summary>
    public abstract class IConCustomException : Exception
    {
        public IConCustomException(String message) : base(message)
        {
        }

        public IConCustomException(String message, Exception innerException) : base(message, innerException)
        {
        }

        protected IConCustomException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Abstract override of oject ToString()
        /// </summary>
        /// <returns></returns>
        public abstract override String ToString();
    }
}
