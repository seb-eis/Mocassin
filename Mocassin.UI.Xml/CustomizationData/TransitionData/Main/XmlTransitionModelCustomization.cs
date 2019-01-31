using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions;

namespace Mocassin.UI.Xml.CustomizationData
{
    /// <summary>
    ///     Serializable object that carries data for customization of <see cref="ITransitionManager" /> rule settings through
    ///     the <see cref="IRuleSetterProvider" /> system
    /// </summary>
    [XmlRoot("TransitionModelCustomization")]
    public class XmlTransitionModelCustomization : XmlModelCustomizationData
    {
        /// <summary>
        ///     Get or set the list of <see cref="XmlKineticTransitionParameterSet" /> objects
        /// </summary>
        [XmlArray("KineticTransitionSets")]
        [XmlArrayItem("KineticTransitionSet")]
        public List<XmlKineticTransitionParameterSet> KineticTransitionParameterSets { get; set; }

        /// <inheritdoc />
        public override void PushToModel(IModelProject modelProject)
        {
            var setterProvider = modelProject.GetManager<ITransitionManager>().QueryPort.Query(x => x.GetRuleSetterProvider());

            foreach (var parameterSet in KineticTransitionParameterSets)
            {
                var transition = modelProject.DataTracker.FindObjectByIndex<IKineticTransition>(parameterSet.TransitionIndex);
                var ruleSetter = setterProvider.GetRuleSetter(transition);
                parameterSet.PushToModel(modelProject, ruleSetter);
            }
        }

        /// <summary>
        /// Create a new <see cref="XmlTransitionModelCustomization"/> by pulling all data from the passed <see cref="IRuleSetterProvider"/>
        /// </summary>
        /// <param name="ruleSetterProvider"></param>
        /// <returns></returns>
        public static XmlTransitionModelCustomization Create(IRuleSetterProvider ruleSetterProvider)
        {
            if (ruleSetterProvider == null) throw new ArgumentNullException(nameof(ruleSetterProvider));

            var obj = new XmlTransitionModelCustomization
            {
                KineticTransitionParameterSets = ruleSetterProvider
                    .GetRuleSetters()
                    .Select(XmlKineticTransitionParameterSet.Create)
                    .ToList()
            };

            return obj;
        }
    }
}