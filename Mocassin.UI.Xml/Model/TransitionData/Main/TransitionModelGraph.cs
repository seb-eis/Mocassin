using System.Collections.Generic;
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
    [XmlRoot("TransitionModel")]
    public class TransitionModelGraph : ModelManagerGraph
    {
        private List<StateExchangePairGraph> stateExchangePairs;
        private List<StateExchangeGroupGraph> stateExchangeGroups;
        private List<AbstractTransitionGraph> abstractTransitions;
        private List<KineticTransitionGraph> kineticTransitions;
        private List<MetropolisTransitionGraph> metropolisTransitions;

        /// <summary>
        ///     Get or set the list of state exchange input objects
        /// </summary>
        [XmlArray("StateChanges")]
        [XmlArrayItem("StateChange")]
        public List<StateExchangePairGraph> StateExchangePairs
        {
            get => stateExchangePairs;
            set => SetProperty(ref stateExchangePairs, value);
        }

        /// <summary>
        ///     Get or set the list of state exchange input objects
        /// </summary>
        [XmlArray("StateChangeGroups")]
        [XmlArrayItem("StateChangeGroup")]
        public List<StateExchangeGroupGraph> StateExchangeGroups
        {
            get => stateExchangeGroups;
            set => SetProperty(ref stateExchangeGroups, value);
        }

        /// <summary>
        ///     Get or set the list of abstract transition input objects
        /// </summary>
        [XmlArray("AbstractTransitions")]
        [XmlArrayItem("AbstractTransition")]
        public List<AbstractTransitionGraph> AbstractTransitions
        {
            get => abstractTransitions;
            set => SetProperty(ref abstractTransitions, value);
        }

        /// <summary>
        ///     Get or set the list of kinetic transition input objects
        /// </summary>
        [XmlArray("KineticTransitions")]
        [XmlArrayItem("KineticTransition")]
        public List<KineticTransitionGraph> KineticTransitions
        {
            get => kineticTransitions;
            set => SetProperty(ref kineticTransitions, value);
        }

        /// <summary>
        ///     Get or set the list of metropolis transition input objects
        /// </summary>
        [XmlArray("MetropolisTransitions")]
        [XmlArrayItem("MetropolisTransition")]
        public List<MetropolisTransitionGraph> MetropolisTransitions
        {
            get => metropolisTransitions;
            set => SetProperty(ref metropolisTransitions, value);
        }

        /// <summary>
        ///     Creates new <see cref="TransitionModelGraph" /> with empty component lists
        /// </summary>
        public TransitionModelGraph()
        {
            StateExchangePairs = new List<StateExchangePairGraph>();
            StateExchangeGroups = new List<StateExchangeGroupGraph>();
            AbstractTransitions = new List<AbstractTransitionGraph>();
            KineticTransitions = new List<KineticTransitionGraph>();
            MetropolisTransitions = new List<MetropolisTransitionGraph>();
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