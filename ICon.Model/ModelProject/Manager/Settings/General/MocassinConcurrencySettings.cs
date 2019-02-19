using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Concurrency settings that specifies how long the program should wait for concurrent read/write access to free
    ///     dependent locks before it throws timeout exceptions
    /// </summary>
    [DataContract]
    [XmlRoot("MocassinConcurrencySettings")]
    public class MocassinConcurrencySettings
    {
        /// <summary>
        ///     The maximum number of locking attempts
        /// </summary>
        [DataMember]
        [XmlAttribute("MaxAttempts")]
        public int MaxAttempts { get; set; }

        /// <summary>
        ///     The interval in between locking attempts
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public TimeSpan AttemptInterval { get; set; }

        /// <summary>
        ///     Get or set the time interval as a string representation
        /// </summary>
        [IgnoreDataMember]
        [XmlAttribute("AttemptInterval")]
        public string AttemptIntervalString
        {
            get => AttemptInterval.ToString();
            set => AttemptInterval = TimeSpan.Parse(value);
        }
    }
}