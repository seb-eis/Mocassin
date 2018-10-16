using System.Runtime.Serialization;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    ///     Basic settings object for transitions that limits the possible input for transitions
    /// </summary>
    [DataContract]
    public class BasicTransitionSettings
    {
        /// <summary>
        ///     The value restriction setting for the number of transitions
        /// </summary>
        [DataMember]
        public ValueSetting<int> TransitionCount { get; set; }

        /// <summary>
        ///     The value restriction setting for the transition length
        /// </summary>
        [DataMember]
        public ValueSetting<int> TransitionLength { get; set; }

        /// <summary>
        ///     The regular expression string pattern for the abstract transition naming restriction
        /// </summary>
        [DataMember]
        public string TransitionStringPattern { get; set; }

        /// <summary>
        ///     Boolean flag to activate automatic filtering of unrecognized rule movement types during rule generation
        /// </summary>
        [DataMember]
        public bool FilterNotRecognizedRuleTypes { get; set; }
    }
}