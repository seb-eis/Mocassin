using System;
using System.Xml.Serialization;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.Base;

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
        ///     Creates a new serializable <see cref="KineticRuleGraph" /> by pulling the required data from the passed
        ///     <see cref="IKineticRule" /> model object interface
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static KineticRuleGraph Create(IKineticRule rule)
        {
            if (rule == null) 
                throw new ArgumentNullException(nameof(rule));

            var obj = new KineticRuleGraph
            {
                RuleIndex = rule.Index,
                AttemptFrequency = rule.AttemptFrequency,
                FinalState = OccupationStateGraph.Create(rule.GetFinalStateOccupation()),
                StartState = OccupationStateGraph.Create(rule.GetStartStateOccupation()),
                TransitionState = OccupationStateGraph.Create(rule.GetTransitionStateOccupation()),
                RuleFlags = rule.MovementFlags.ToString()
            };

            return obj;
        }
    }
}