using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

namespace Mocassin.Framework.Messaging
{
    /// <summary>
    /// Abstract push notification message base class for all messages within ICon that are designed to be distributed and handled through the callback messaging system
    /// </summary>
    public abstract class PushMessage
    {
        /// <summary>
        /// Creates a new operation message with the minimal information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shortInfo"></param>
        protected PushMessage(object sender, string shortInfo)
        {
            Sender = sender;
            ShortInfo = shortInfo;
        }

        /// <summary>
        /// The instance that the operation message was send from
        /// </summary>
        [JsonIgnore]
        public object Sender { get; }

        /// <summary>
        /// Basic short message describing the contents of the model message
        /// </summary>
        public string ShortInfo { get; set; }

        /// <summary>
        /// Creates a single string from all the message contents
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
