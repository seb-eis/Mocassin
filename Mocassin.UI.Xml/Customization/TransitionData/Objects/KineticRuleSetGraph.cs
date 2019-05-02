using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Model;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable object that carries data for customization of <see cref="IKineticTransition" /> rule settings through
    ///     the <see cref="IRuleSetterProvider" /> system
    /// </summary>
    [XmlRoot("TransitionModelParametrization")]
    public class KineticRuleSetGraph : ProjectObjectGraph
    {
        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}"/> for the affiliated <see cref="KineticTransition"/> 
        /// </summary>
        [XmlElement("Transition")]
        public ModelObjectReferenceGraph<KineticTransition> Transition { get; set; }

        /// <summary>
        ///     Get or set the transition index of the affiliated <see cref="IKineticTransition" />
        /// </summary>
        [XmlAttribute("AutoId")]
        public int TransitionIndex { get; set; }

        /// <summary>
        ///     Get or set the list of affiliated <see cref="KineticRuleGraph" /> objects to customize the rules
        /// </summary>
        [XmlArray("TransitionRules")]
        [XmlArrayItem("TransitionRule")]
        public List<KineticRuleGraph> KineticRules { get; set; }

        /// <summary>
        ///     Set all data on the passed <see cref="IKineticRuleSetter" /> and push the values to the affiliated
        ///     <see cref="Mocassin.Model.ModelProject.IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="ruleSetter"></param>
        public void PushToModel(IModelProject modelProject, IKineticRuleSetter ruleSetter)
        {
            var setData = KineticRules.Select(x => (x, modelProject.DataTracker.FindObjectByIndex<IKineticRule>(x.RuleIndex)));

            foreach (var (xmlRule, rule) in setData)
                ruleSetter.SetAttemptFrequency(rule, xmlRule.AttemptFrequency);

            ruleSetter.PushData();
        }

        /// <summary>
        ///     Creates a new <see cref="KineticRuleSetGraph" /> by pulling all data from the passed
        ///     <see cref="IKineticRuleSetter" /> and <see cref="ProjectModelGraph"/> parent
        /// </summary>
        /// <param name="ruleSetter"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static KineticRuleSetGraph Create(IKineticRuleSetter ruleSetter, ProjectModelGraph parent)
        {
            if (ruleSetter == null) throw new ArgumentNullException(nameof(ruleSetter));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var transitionGraph = parent.TransitionModelGraph.KineticTransitions.Single(x => x.Key == ruleSetter.KineticTransition.Key);

            var obj = new KineticRuleSetGraph
            {
                Name = $"Kinetic.Rule.Set.{transitionGraph}",
                TransitionIndex = ruleSetter.KineticTransition.Index,
                Transition = new ModelObjectReferenceGraph<KineticTransition> {TargetGraph = transitionGraph},
                KineticRules = ruleSetter.KineticRules.Select(x => KineticRuleGraph.Create(x, parent)).ToList()
            };

            return obj;
        }
    }
}