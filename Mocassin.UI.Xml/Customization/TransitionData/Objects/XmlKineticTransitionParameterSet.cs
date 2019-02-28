using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable object that carries data for customization of <see cref="IKineticTransition" /> rule settings through
    ///     the <see cref="IRuleSetterProvider" /> system
    /// </summary>
    [XmlRoot("TransitionModelParametrization")]
    public class XmlKineticTransitionParameterSet
    {
        /// <summary>
        ///     Get or set the key of the affiliated <see cref="IKineticTransition" />
        /// </summary>
        [XmlAttribute("Transition")]
        public string TransitionKey { get; set; }

        /// <summary>
        ///     Get or set the transition index of the affiliated <see cref="IKineticTransition" />
        /// </summary>
        [XmlAttribute("AutoId")]
        public int TransitionIndex { get; set; }

        /// <summary>
        ///     Get or set the list of affiliated <see cref="XmlKineticRule" /> objects to customize the rules
        /// </summary>
        [XmlArray("TransitionRules")]
        [XmlArrayItem("TransitionRule")]
        public List<XmlKineticRule> KineticRules { get; set; }

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
        ///     Creates a new <see cref="XmlKineticTransitionParameterSet" /> by pulling all data from the passed
        ///     <see cref="IKineticRuleSetter" />
        /// </summary>
        /// <param name="ruleSetter"></param>
        /// <returns></returns>
        public static XmlKineticTransitionParameterSet Create(IKineticRuleSetter ruleSetter)
        {
            if (ruleSetter == null) throw new ArgumentNullException(nameof(ruleSetter));

            var obj = new XmlKineticTransitionParameterSet
            {
                TransitionIndex = ruleSetter.KineticTransition.Index,
                TransitionKey = ruleSetter.KineticTransition.Key,
                KineticRules = ruleSetter.KineticRules.Select(XmlKineticRule.Create).ToList()
            };

            return obj;
        }
    }
}