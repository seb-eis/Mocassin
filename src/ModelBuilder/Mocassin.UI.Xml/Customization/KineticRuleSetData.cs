﻿using System;
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
    ///     Serializable object that carries data for customization of <see cref="IKineticTransition" /> rule settings through
    ///     the <see cref="IRuleSetterProvider" /> system
    /// </summary>
    [XmlRoot("TransitionModelParametrization")]
    public class KineticRuleSetData : ProjectDataObject, IDuplicable<KineticRuleSetData>
    {
        private ObservableCollection<KineticRuleData> kineticRules;
        private ModelObjectReference<KineticTransition> transition;
        private int transitionIndex;
        private int transitionCount;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReference{T}" /> for the affiliated <see cref="KineticTransition" />
        /// </summary>
        [XmlElement("Transition")]
        public ModelObjectReference<KineticTransition> Transition
        {
            get => transition;
            set => SetProperty(ref transition, value);
        }

        /// <summary>
        ///     Get or set the number of transitions per unit cell
        /// </summary>
        [XmlElement("TransitionCount")]
        public int TransitionCount
        {
            get => transitionCount;
            set => SetProperty(ref transitionCount, value);
        }

        /// <summary>
        ///     Get or set the transition index of the affiliated <see cref="IKineticTransition" />
        /// </summary>
        [XmlAttribute("AutoId")]
        public int TransitionIndex
        {
            get => transitionIndex;
            set => SetProperty(ref transitionIndex, value);
        }

        /// <summary>
        ///     Get or set the list of affiliated <see cref="KineticRuleData" /> objects to customize the rules
        /// </summary>
        [XmlArray("TransitionRules"), XmlArrayItem("TransitionRule")]
        public ObservableCollection<KineticRuleData> KineticRules
        {
            get => kineticRules;
            set => SetProperty(ref kineticRules, value);
        }

        /// <inheritdoc />
        public KineticRuleSetData Duplicate()
        {
            var copy = new KineticRuleSetData
            {
                Name = Name,
                transition = transition.Duplicate(),
                transitionIndex = transitionIndex,
                kineticRules = kineticRules.Select(x => x.Duplicate()).ToObservableCollection()
            };
            return copy;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate() => Duplicate();

        /// <summary>
        ///     Set all data on the passed <see cref="IKineticRuleSetter" /> and push the values to the affiliated
        ///     <see cref="Mocassin.Model.ModelProject.IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="ruleSetter"></param>
        public void PushToModel(IModelProject modelProject, IKineticRuleSetter ruleSetter)
        {
            var setData = KineticRules.Select(x => (x, modelProject.DataTracker.FindObject<IKineticRule>(x.RuleIndex)));

            foreach (var (xmlRule, rule) in setData)
                ruleSetter.SetAttemptFrequency(rule, xmlRule.AttemptFrequency);

            ruleSetter.PushData();
        }

        /// <summary>
        ///     Creates a new <see cref="KineticRuleSetData" /> by pulling all data from the passed
        ///     <see cref="IKineticRuleSetter" /> and <see cref="ProjectModelData" /> parent
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="ruleSetter"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static KineticRuleSetData Create(IModelProject modelProject, IKineticRuleSetter ruleSetter, ProjectModelData parent)
        {
            if (ruleSetter == null) throw new ArgumentNullException(nameof(ruleSetter));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var transitionData = parent.TransitionModelData.KineticTransitions.Single(x => x.Key == ruleSetter.KineticTransition.Key);
            var transitionCount = modelProject.Manager<ITransitionManager>().DataAccess.Query(x => x.GetKineticMappingList(ruleSetter.KineticTransition.Index)).Count;
            
            var obj = new KineticRuleSetData
            {
                Name = $"Kinetic.Rule.Set.{transitionData}",
                TransitionIndex = ruleSetter.KineticTransition.Index,
                TransitionCount = transitionCount,
                Transition = new ModelObjectReference<KineticTransition> {Target = transitionData},
                KineticRules = ruleSetter.KineticRules.Select((x, i) => KineticRuleData.Create(x, parent, i)).ToObservableCollection()
            };

            return obj;
        }
    }
}