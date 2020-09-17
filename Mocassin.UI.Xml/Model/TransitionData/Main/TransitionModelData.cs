using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.TransitionModel
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the
    ///     <see cref="Mocassin.Model.Transitions.ITransitionManager" />
    ///     system
    /// </summary>
    [XmlRoot]
    public class TransitionModelData : ModelManagerData
    {
        private ObservableCollection<AbstractTransitionData> abstractTransitions;
        private ObservableCollection<KineticTransitionData> kineticTransitions;
        private ObservableCollection<MetropolisTransitionData> metropolisTransitions;
        private ObservableCollection<StateExchangeGroupData> stateExchangeGroups;
        private ObservableCollection<StateExchangePairData> stateExchangePairs;

        /// <summary>
        ///     Get or set the list of state exchange input objects
        /// </summary>
        [XmlArray]
        public ObservableCollection<StateExchangePairData> StateExchangePairs
        {
            get => stateExchangePairs;
            set => SetProperty(ref stateExchangePairs, value);
        }

        /// <summary>
        ///     Get or set the list of state exchange input objects
        /// </summary>
        [XmlArray]
        public ObservableCollection<StateExchangeGroupData> StateExchangeGroups
        {
            get => stateExchangeGroups;
            set => SetProperty(ref stateExchangeGroups, value);
        }

        /// <summary>
        ///     Get or set the list of abstract transition input objects
        /// </summary>
        [XmlArray]
        public ObservableCollection<AbstractTransitionData> AbstractTransitions
        {
            get => abstractTransitions;
            set => SetProperty(ref abstractTransitions, value);
        }

        /// <summary>
        ///     Get or set the list of kinetic transition input objects
        /// </summary>
        [XmlArray]
        public ObservableCollection<KineticTransitionData> KineticTransitions
        {
            get => kineticTransitions;
            set => SetProperty(ref kineticTransitions, value);
        }

        /// <summary>
        ///     Get or set the list of metropolis transition input objects
        /// </summary>
        [XmlArray]
        public ObservableCollection<MetropolisTransitionData> MetropolisTransitions
        {
            get => metropolisTransitions;
            set => SetProperty(ref metropolisTransitions, value);
        }

        /// <summary>
        ///     Creates new <see cref="TransitionModelData" /> with empty component lists
        /// </summary>
        public TransitionModelData()
        {
            StateExchangePairs = new ObservableCollection<StateExchangePairData>();
            StateExchangeGroups = new ObservableCollection<StateExchangeGroupData>();
            AbstractTransitions = new ObservableCollection<AbstractTransitionData>();
            KineticTransitions = new ObservableCollection<KineticTransitionData>();
            MetropolisTransitions = new ObservableCollection<MetropolisTransitionData>();
        }

        /// <inheritdoc />
        public override IEnumerable<IModelParameter> GetInputParameters()
        {
            yield break;
        }

        /// <inheritdoc />
        public override IEnumerable<IModelObject> GetInputObjects()
        {
            return StateExchangePairs.Select(x => x.GetInputObject())
                                     .Concat(StateExchangeGroups.Select(x => x.GetInputObject()))
                                     .Concat(AbstractTransitions.Select(x => x.GetInputObject()))
                                     .Concat(KineticTransitions.Select(x => x.GetInputObject()))
                                     .Concat(MetropolisTransitions.Select(x => x.GetInputObject()));
        }
    }
}