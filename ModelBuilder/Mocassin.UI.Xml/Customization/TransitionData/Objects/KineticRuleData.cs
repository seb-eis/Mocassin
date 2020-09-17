using System;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Model;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Transitions.IKineticRule" /> model object creation and
    ///     manipulation
    /// </summary>
    [XmlRoot("KineticRule")]
    public class KineticRuleData : ProjectDataObject, IDuplicable<KineticRuleData>
    {
        private double attemptFrequency = DefaultAttemptFrequency;
        private int dependencyRuleCount;
        private OccupationStateData finalState;
        private string ruleFlags;
        private int ruleIndex;
        private OccupationStateData startState;
        private OccupationStateData transitionState;

        /// <summary>
        ///     Get or set the default value used for the <see cref="AttemptFrequency" />. Standard ist 1.0e13 Hz
        /// </summary>
        public static double DefaultAttemptFrequency { get; set; } = 1.0e13;

        /// <summary>
        ///     Get or set the attempt frequency value of the rule in [Hz]
        /// </summary>
        [XmlAttribute("AttemptFrequency")]
        public double AttemptFrequency
        {
            get => attemptFrequency;
            set => SetProperty(ref attemptFrequency, value);
        }

        /// <summary>
        ///     Get or set the rule flag info string
        /// </summary>
        [XmlAttribute("Info")]
        public string RuleFlags
        {
            get => ruleFlags;
            set => SetProperty(ref ruleFlags, value);
        }

        /// <summary>
        ///     Get or set the internal index of the affected rule
        /// </summary>
        [XmlAttribute("AutoId")]
        public int RuleIndex
        {
            get => ruleIndex;
            set => SetProperty(ref ruleIndex, value);
        }

        /// <summary>
        ///     Get or set the occupation state of the start state
        /// </summary>
        [XmlElement("StartState")]
        public OccupationStateData StartState
        {
            get => startState;
            set => SetProperty(ref startState, value);
        }

        /// <summary>
        ///     Get or set the occupation state of the transition state
        /// </summary>
        [XmlElement("TransitionState")]
        public OccupationStateData TransitionState
        {
            get => transitionState;
            set => SetProperty(ref transitionState, value);
        }

        /// <summary>
        ///     Get or set the occupation state of the final state
        /// </summary>
        [XmlElement("FinalState")]
        public OccupationStateData FinalState
        {
            get => finalState;
            set => SetProperty(ref finalState, value);
        }

        /// <summary>
        ///     Get or set the number of dependency rules
        /// </summary>
        [XmlElement("DependencyRuleCount")]
        public int DependencyRuleCount
        {
            get => dependencyRuleCount;
            set => SetProperty(ref dependencyRuleCount, value);
        }

        /// <inheritdoc />
        public KineticRuleData Duplicate()
        {
            var copy = new KineticRuleData
            {
                Name = Name,
                attemptFrequency = attemptFrequency,
                ruleFlags = ruleFlags,
                ruleIndex = ruleIndex,
                dependencyRuleCount = dependencyRuleCount,
                startState = startState.Duplicate(),
                transitionState = transitionState.Duplicate(),
                finalState = finalState.Duplicate()
            };
            return copy;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate() => Duplicate();

        /// <summary>
        ///     Creates a new serializable <see cref="KineticRuleData" /> by pulling the required data from the passed
        ///     <see cref="IKineticRule" /> and <see cref="ProjectModelData" /> parent
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="parent"></param>
        /// <param name="localIndex"></param>
        /// <returns></returns>
        public static KineticRuleData Create(IKineticRule rule, ProjectModelData parent, int localIndex)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var obj = new KineticRuleData
            {
                Name = $"Rule.{localIndex}",
                DependencyRuleCount = rule.GetDependentRules().Count(),
                RuleIndex = rule.Index,
                AttemptFrequency = rule.AttemptFrequency,
                FinalState = OccupationStateData.Create(rule.GetFinalStateOccupation(), parent.ParticleModelData.Particles),
                StartState = OccupationStateData.Create(rule.GetStartStateOccupation(), parent.ParticleModelData.Particles),
                TransitionState = OccupationStateData.Create(rule.GetTransitionStateOccupation(), parent.ParticleModelData.Particles),
                RuleFlags = rule.MovementFlags.ToString()
            };

            return obj;
        }

        /// <summary>
        ///     Check if all occupation states are equal to the states on the provided <see cref="KineticRuleSetData" />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool HasEqualStates(KineticRuleData other) =>
            other != null &&
            (ReferenceEquals(this, other) ||
             StartState.HasEqualState(other.StartState) &&
             TransitionState.HasEqualState(other.TransitionState) &&
             FinalState.HasEqualState(other.FinalState));
    }
}