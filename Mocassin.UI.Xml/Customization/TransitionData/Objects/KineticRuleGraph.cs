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
    public class KineticRuleGraph : ProjectObjectGraph, IDuplicable<KineticRuleGraph>
    {
        /// <summary>
        ///     Get or set the default value used for the <see cref="AttemptFrequency"/>. Standard ist 1.0e13 Hz
        /// </summary>
        public static double DefaultAttemptFrequency { get; set; } = 1.0e13;

        private double attemptFrequency = DefaultAttemptFrequency;
        private string ruleFlags;
        private int ruleIndex;
        private OccupationStateGraph startState;
        private OccupationStateGraph transitionState;
        private OccupationStateGraph finalState;
        private int dependencyRuleCount;

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
        public OccupationStateGraph StartState
        {
            get => startState;
            set => SetProperty(ref startState, value);
        }

        /// <summary>
        ///     Get or set the occupation state of the transition state
        /// </summary>
        [XmlElement("TransitionState")]
        public OccupationStateGraph TransitionState
        {
            get => transitionState;
            set => SetProperty(ref transitionState, value);
        }

        /// <summary>
        ///     Get or set the occupation state of the final state
        /// </summary>
        [XmlElement("FinalState")]
        public OccupationStateGraph FinalState
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

        /// <summary>
        ///     Creates a new serializable <see cref="KineticRuleGraph" /> by pulling the required data from the passed
        ///     <see cref="IKineticRule" /> and <see cref="ProjectModelGraph" /> parent
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static KineticRuleGraph Create(IKineticRule rule, ProjectModelGraph parent)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var obj = new KineticRuleGraph
            {
                Name = $"Rule.{rule.Index}",
                DependencyRuleCount = rule.GetDependentRules().Count(),
                RuleIndex = rule.Index,
                AttemptFrequency = rule.AttemptFrequency,
                FinalState = OccupationStateGraph.Create(rule.GetFinalStateOccupation(), parent.ParticleModelGraph.Particles),
                StartState = OccupationStateGraph.Create(rule.GetStartStateOccupation(), parent.ParticleModelGraph.Particles),
                TransitionState = OccupationStateGraph.Create(rule.GetTransitionStateOccupation(), parent.ParticleModelGraph.Particles),
                RuleFlags = rule.MovementFlags.ToString()
            };

            return obj;
        }

        /// <inheritdoc />
        public KineticRuleGraph Duplicate()
        {
            var copy = new KineticRuleGraph
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

        /// <summary>
        ///     Check if all occupation states are equal to the states on the provided <see cref="KineticRuleSetGraph"/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool HasEqualStates(KineticRuleGraph other)
        {
            return other != null && 
                   (ReferenceEquals(this, other) ||
                    StartState.HasEqualState(other.StartState) && 
                    TransitionState.HasEqualState(other.TransitionState) &&
                    FinalState.HasEqualState(other.FinalState));
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }
    }
}