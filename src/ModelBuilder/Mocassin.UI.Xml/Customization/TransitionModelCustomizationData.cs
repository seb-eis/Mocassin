using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Transitions;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.Customization
{
    /// <summary>
    ///     Serializable object that carries data for customization of <see cref="ITransitionManager" /> rule settings through
    ///     the <see cref="IRuleSetterProvider" /> system
    /// </summary>
    [XmlRoot("TransitionModelCustomization")]
    public class TransitionModelCustomizationData : ModelCustomizationData, IDuplicable<TransitionModelCustomizationData>
    {
        private ObservableCollection<KineticRuleSetData> kineticTransitionParameterSets;

        /// <summary>
        ///     Get or set the list of <see cref="KineticRuleSetData" /> objects
        /// </summary>
        [XmlArray("KineticTransitionSets"), XmlArrayItem("KineticTransitionSet")]
        public ObservableCollection<KineticRuleSetData> KineticTransitionParameterSets
        {
            get => kineticTransitionParameterSets;
            set => SetProperty(ref kineticTransitionParameterSets, value);
        }

        /// <inheritdoc />
        public TransitionModelCustomizationData Duplicate()
        {
            var copy = new TransitionModelCustomizationData
            {
                Name = Name,
                kineticTransitionParameterSets = kineticTransitionParameterSets.Select(x => x.Duplicate()).ToObservableCollection()
            };
            return copy;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate() => Duplicate();

        /// <inheritdoc />
        public override void PushToModel(IModelProject modelProject)
        {
            var setterProvider = modelProject.Manager<ITransitionManager>().DataAccess.Query(x => x.GetRuleSetterProvider());

            foreach (var parameterSet in KineticTransitionParameterSets)
            {
                var transition = modelProject.DataTracker.FindObject<IKineticTransition>(parameterSet.TransitionIndex);
                var ruleSetter = setterProvider.GetRuleSetter(transition);
                parameterSet.PushToModel(modelProject, ruleSetter);
            }
        }

        /// <summary>
        ///     Create a new <see cref="TransitionModelCustomizationData" /> by pulling all data from the passed
        ///     <see cref="IModelProject" /> and <see cref="ProjectModelData" /> parent
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static TransitionModelCustomizationData Create(IModelProject modelProject, ProjectModelData parent)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var ruleSetterProvider = modelProject.Manager<ITransitionManager>().DataAccess.Query(x => x.GetRuleSetterProvider());
            
            var obj = new TransitionModelCustomizationData
            {
                KineticTransitionParameterSets = ruleSetterProvider
                                                 .GetRuleSetters()
                                                 .Select(x => KineticRuleSetData.Create(modelProject, x, parent))
                                                 .ToObservableCollection()
            };

            return obj;
        }
    }
}