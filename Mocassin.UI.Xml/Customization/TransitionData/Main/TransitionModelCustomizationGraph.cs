using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Model;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable object that carries data for customization of <see cref="ITransitionManager" /> rule settings through
    ///     the <see cref="IRuleSetterProvider" /> system
    /// </summary>
    [XmlRoot("TransitionModelCustomization")]
    public class TransitionModelCustomizationGraph : ModelCustomizationEntity, IDuplicable<TransitionModelCustomizationGraph>
    {
        private ObservableCollection<KineticRuleSetGraph> kineticTransitionParameterSets;

        /// <summary>
        ///     Get or set the list of <see cref="KineticRuleSetGraph" /> objects
        /// </summary>
        [XmlArray("KineticTransitionSets")]
        [XmlArrayItem("KineticTransitionSet")]
        public ObservableCollection<KineticRuleSetGraph> KineticTransitionParameterSets
        {
            get => kineticTransitionParameterSets;
            set => SetProperty(ref kineticTransitionParameterSets, value);
        }

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
        ///     Create a new <see cref="TransitionModelCustomizationGraph" /> by pulling all data from the passed
        ///     <see cref="IRuleSetterProvider" /> and <see cref="ProjectModelGraph" /> parent
        /// </summary>
        /// <param name="ruleSetterProvider"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static TransitionModelCustomizationGraph Create(IRuleSetterProvider ruleSetterProvider, ProjectModelGraph parent)
        {
            if (ruleSetterProvider == null) throw new ArgumentNullException(nameof(ruleSetterProvider));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var obj = new TransitionModelCustomizationGraph
            {
                KineticTransitionParameterSets = ruleSetterProvider
                    .GetRuleSetters()
                    .Select(x => KineticRuleSetGraph.Create(x, parent))
                    .ToObservableCollection()
            };

            return obj;
        }

        /// <inheritdoc />
        public TransitionModelCustomizationGraph Duplicate()
        {
            var copy = new TransitionModelCustomizationGraph
            {
                Name = Name,
                kineticTransitionParameterSets = kineticTransitionParameterSets.Select(x => x.Duplicate()).ToObservableCollection()
            };
            return copy;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }
    }
}