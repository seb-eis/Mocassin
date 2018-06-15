using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Concurrency settings that specifiy how long the program should wait for concurrent read/write access to free dependent locks before it throws timeout exceptions
    /// </summary>
    [Serializable]
    [DataContract]
    public class BasicConcurrencySettings
    {
        /// <summary>
        /// The maximum number of locking attempts
        /// </summary>
        [DataMember(Name ="Attempts")]
        public Int32 MaxAttempts { get; set; }

        /// <summary>
        /// The interval in between locking attempts
        /// </summary>
        [DataMember(Name ="Interval")]
        public TimeSpan AttemptInterval { get; set; }
    }
}
