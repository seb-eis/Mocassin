using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mocassin.Framework.Messaging
{
    /// <summary>
    ///     Abstract push notification message base class for all messages within ICon that are designed to be distributed and
    ///     handled through the callback messaging system
    /// </summary>
    public abstract class PushMessage
    {
        /// <summary>
        ///     The instance that the operation message was send from
        /// </summary>
        [JsonIgnore]
        public object Sender { get; }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> sequence of <see cref="string" /> that describe details fo the message
        /// </summary>
        [JsonIgnore]
        public abstract IEnumerable<string> DetailSequence { get; }

        /// <summary>
        ///     Get an indented JSON representation of the message details
        /// </summary>
        [JsonIgnore]
        public string IndentedDetailsJson => DetailsToJson(Formatting.Indented);

        /// <summary>
        ///     Get an default JSON representation of the message details
        /// </summary>
        [JsonIgnore]
        public string DetailsJson => DetailsToJson();

        /// <summary>
        ///     Basic short message describing the contents of the model message
        /// </summary>
        public string ShortInfo { get; set; }

        /// <summary>
        ///     Get the <see cref="DateTime" /> when the message was created
        /// </summary>
        public DateTime TimeStamp { get; }

        /// <summary>
        ///     Creates a new operation message with the minimal information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shortInfo"></param>
        protected PushMessage(object sender, string shortInfo)
        {
            Sender = sender;
            ShortInfo = shortInfo;
            TimeStamp = DateTime.Now;
        }

        /// <summary>
        ///     Creates a single string from all the message contents
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }

        /// <summary>
        ///     Converts the message details to a JSON string
        /// </summary>
        /// <param name="formatting"></param>
        /// <returns></returns>
        public string DetailsToJson(Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(DetailSequence, formatting);
        }
    }
}