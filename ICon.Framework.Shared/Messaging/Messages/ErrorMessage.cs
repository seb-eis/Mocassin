using System;
using System.Collections.Generic;
using System.Text;

namespace Mocassin.Framework.Messaging
{
    /// <summary>
    /// Exception message class that carries error information including the thrown exceptions (Only to be used when program errors occure)
    /// </summary>
    public class ErrorMessage : PushMessage
    {
        /// <summary>
        /// The caught exception
        /// </summary>
        public Exception Exception { get; set; }

        /// <inheritdoc />
        public ErrorMessage(object sender, string shortInfo) : base(sender, shortInfo)
        {
        }
    }
}
