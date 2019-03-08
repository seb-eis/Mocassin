using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable object that carries data for customization of <see cref="ITransitionManager" /> rule settings through
    ///     the <see cref="IRuleSetterProvider" /> system
    /// </summary>
    [XmlRoot("TransitionModelCustomization")]
    public class TransitionModelCustomizationEntity : ModelCustomizationEntity
    {
        /// <summary>
        ///     Get or set the list of <see cref="KineticRuleSetGraph" /> objects
        /// </summary>
        [XmlArray("KineticTransitionSets")]
        [XmlArrayItem("KineticTransitionSet")]
        public List<KineticRuleSetGraph> KineticTransitionParameterSets { get; set; }

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
        ///     Create a new <see cref="TransitionModelCustomizationEntity" /> by pulling all data from the passed
        ///     <see cref="IRuleSetterProvider" />
        /// </summary>
        /// <param name="ruleSetterProvider"></param>
        /// <returns></returns>
        public static TransitionModelCustomizationEntity Create(IRuleSetterProvider ruleSetterProvider)
        {
            if (ruleSetterProvider == null) throw new ArgumentNullException(nameof(ruleSetterProvider));

            var obj = new TransitionModelCustomizationEntity
            {
                KineticTransitionParameterSets = ruleSetterProvider
                    .GetRuleSetters()
                    .Select(KineticRuleSetGraph.Create)
                    .ToList()
            };

            return obj;
        }
    }
}