using System.Runtime.Serialization;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Basic settings object for transitions that limits the possible input for transitions
    /// </summary>
    [DataContract]
    [ModuleSettings(typeof(ITransitionManager))]
    public class MocassinTransitionSettings : MocassinModuleSettings
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

        /// <inheritdoc />
        public override void InitAsDefault()
        {
            TransitionCount = new ValueSetting<int>("Transition Count", 0, 100);
            TransitionLength = new ValueSetting<int>("Transition Length", 2, 8);
            TransitionStringPattern = "^[a-zA-Z0-9\\+\\-\\(\\)]{2,100}$";
            FilterNotRecognizedRuleTypes = true;
        }
    }
}