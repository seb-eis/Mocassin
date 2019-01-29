using System;
using System.Xml.Serialization;
using Mocassin.Model.Transitions;

namespace Mocassin.UI.Xml.CustomizationData
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Transitions.IKineticRule" /> model object creation and
    ///     manipulation
    /// </summary>
    [XmlRoot("KineticTransitionRule")]
    public class XmlKineticRule
    {
        /// <summary>
        ///     Get or set the key of the transition the rule is valid for
        /// </summary>
        [XmlAttribute("Transition")]
        public string TransitionKey { get; set; }

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
        public XmlOccupationState StartState { get; set; }

        /// <summary>
        ///     Get or set the occupation state of the transition state
        /// </summary>
        [XmlElement("TransitionState")]
        public XmlOccupationState TransitionState { get; set; }

        /// <summary>
        ///     Get or set the occupation state of the final state
        /// </summary>
        [XmlElement("FinalState")]
        public XmlOccupationState FinalState { get; set; }

        /// <summary>
        ///     Creates a new serializable <see cref="XmlKineticRule" /> by pulling the required data from the passed
        ///     <see cref="IKineticRule" /> model object interface
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static XmlKineticRule Create(IKineticRule rule)
        {
            if (rule == null) 
                throw new ArgumentNullException(nameof(rule));

            var obj = new XmlKineticRule
            {
                TransitionKey = rule.Transition.Key,
                RuleIndex = rule.Index,
                AttemptFrequency = rule.AttemptFrequency,
                FinalState = XmlOccupationState.Create(rule.GetFinalStateOccupation()),
                StartState = XmlOccupationState.Create(rule.GetStartStateOccupation()),
                TransitionState = XmlOccupationState.Create(rule.GetTransitionStateOccupation()),
                RuleFlags = rule.MovementFlags.ToString()
            };

            return obj;
        }
    }
}