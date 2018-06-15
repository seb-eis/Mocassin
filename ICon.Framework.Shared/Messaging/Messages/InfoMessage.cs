using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Messaging
{
    /// <summary>
    /// ICon information message class that carries information massages or warnings from the framework or model libraries (non critical messages)
    /// </summary>
    public class InfoMessage : PushMessage
    {
        /// <summary>
        /// Creates new info message with the minimum amount of information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shortInfo"></param>
        public InfoMessage(object sender, string shortInfo) : base(sender, shortInfo)
        {

        }

        /// <summary>
        /// Contains additional information and explanations 
        /// </summary>
        public List<string> Details { get; set; }
    }
}
