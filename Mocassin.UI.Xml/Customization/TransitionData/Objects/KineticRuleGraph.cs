using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Model;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Transitions.IKineticRule" /> model object creation and
    ///     manipulation
    /// </summary>
    [XmlRoot("KineticRule")]
    public class KineticRuleGraph : ProjectObjectGraph
    {
        /// <summary>
        ///     Get or set the attempt frequency value of the rule in [Hz]
        /// </summary>
        [XmlAttribute("AttemptFrequency")]
        public double AttemptFrequency { get; set; }

        /// <summary>
        /// Get or set the rule flag info string
        /// </summary>
        [XmlAttribute("Info")]
        public string RuleFlags { get; set; }

        /// <summary>
        ///     Get or set the internal index of the affected rule
        /// </summary>
        [XmlAttribute("AutoId")]
        public int RuleIndex { get; set; }

        /// <summary>
        ///     Get or set the occupation state of the start state
        /// </summary>
        [XmlElement("StartState")]
        public OccupationStateGraph StartState { get; set; }

        /// <summary>
        ///     Get or set the occupation state of the transition state
        /// </summary>
        [XmlElement("TransitionState")]
        public OccupationStateGraph TransitionState { get; set; }

        /// <summary>
        ///     Get or set the occupation state of the final state
        /// </summary>
        [XmlElement("FinalState")]
        public OccupationStateGraph FinalState { get; set; }

        /// <summary>
        ///     Get or set the number of dependency rules
        /// </summary>
        [XmlElement("DependencyRuleCount")]
        public int DependencyRuleCount { get; set; }

        /// <summary>
        ///     Creates a new serializable <see cref="KineticRuleGraph" /> by pulling the required data from the passed
        ///     <see cref="IKineticRule" /> and <see cref="ProjectModelGraph"/> parent
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
    }
}