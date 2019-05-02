using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.Model;

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
        ///     <see cref="IRuleSetterProvider" /> and <see cref="ProjectModelGraph"/> parent
        /// </summary>
        /// <param name="ruleSetterProvider"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static TransitionModelCustomizationEntity Create(IRuleSetterProvider ruleSetterProvider, ProjectModelGraph parent)
        {
            if (ruleSetterProvider == null) throw new ArgumentNullException(nameof(ruleSetterProvider));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var obj = new TransitionModelCustomizationEntity
            {
                KineticTransitionParameterSets = ruleSetterProvider
                    .GetRuleSetters()
                    .Select(x => KineticRuleSetGraph.Create(x, parent))
                    .ToList()
            };

            return obj;
        }
    }
}