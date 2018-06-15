using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Basic settings object for transitions that limits the possible input for transitions
    /// </summary>
    [Serializable]
    [DataContract(Name ="TransitionSettings")]
    public class BasicTransitionSettings
    {
        /// <summary>
        /// The maximum number of allowed transitions
        /// </summary>
        [DataMember(Name = "MasTransitionCount")]
        public int MaxTransitionCount { get; set; }

        /// <summary>
        /// The minimal transition length (usually at least two positions are required)
        /// </summary>
        [DataMember(Name ="MinTransitionLength")]
        public int MinTransitionLength { get; set; }

        /// <summary>
        /// The maximum number of allowed transtion steps
        /// </summary>
        [DataMember(Name = "MaxTransitionLength")]
        public int MaxTransitionLength { get; set; }

        /// <summary>
        /// The regular expression string for the abstract transition naming restriction
        /// </summary>
        [DataMember(Name = "AbstractTransitionNameRegex")]
        public string AbstractTransitionNameRegex { get; set; }
    }
}
