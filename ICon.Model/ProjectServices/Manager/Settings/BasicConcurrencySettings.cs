using System;
using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    ///     Concurrency settings that specifies how long the program should wait for concurrent read/write access to free
    ///     dependent locks before it throws timeout exceptions
    /// </summary>
    [DataContract]
    public class BasicConcurrencySettings
    {
        /// <summary>
        ///     The maximum number of locking attempts
        /// </summary>
        [DataMember]
        public int MaxAttempts { get; set; }

        /// <summary>
        ///     The interval in between locking attempts
        /// </summary>
        [DataMember]
        public TimeSpan AttemptInterval { get; set; }
    }
}