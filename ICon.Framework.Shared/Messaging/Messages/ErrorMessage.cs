using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Messaging
{
    /// <summary>
    /// ICon exception message class that carries error information including the catched thrown exceptions (Only to be used when program errors occure)
    /// </summary>
    public class ErrorMessage : PushMessage
    {
        /// <summary>
        /// The catched exception
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Creates new error mesagge with the minimal information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shortInfo"></param>
        public ErrorMessage(object sender, string shortInfo) : base(sender, shortInfo)
        {
        }
    }
}
