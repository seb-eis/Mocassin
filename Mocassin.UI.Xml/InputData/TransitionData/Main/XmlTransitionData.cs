using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.TransitionData
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the
    ///     <see cref="Mocassin.Model.Transitions.ITransitionManager" />
    ///     system
    /// </summary>
    [XmlRoot("TransitionModel")]
    public class XmlTransitionData : XmlProjectManagerData
    {
        /// <summary>
        ///     Get or set the list of state exchange input objects
        /// </summary>
        [XmlArray("StateChanges")]
        [XmlArrayItem("StateChange")]
        public List<XmlStateExchangePair> StateExchangePairs { get; set; }

        /// <summary>
        ///     Get or set the list of state exchange input objects
        /// </summary>
        [XmlArray("StateChangeGroups")]
        [XmlArrayItem("StateChangeGroup")]
        public List<XmlStateExchangeGroup> StateExchangeGroups { get; set; }

        /// <summary>
        ///     Get or set the list of abstract transition input objects
        /// </summary>
        [XmlArray("AbstractTransitions")]
        [XmlArrayItem("AbstractTransition")]
        public List<XmlAbstractTransition> AbstractTransitions { get; set; }

        /// <summary>
        ///     Get or set the list of kinetic transition input objects
        /// </summary>
        [XmlArray("KineticTransitions")]
        [XmlArrayItem("KineticTransition")]
        public List<XmlKineticTransition> KineticTransitions { get; set; }

        /// <summary>
        ///     Get or set the list of metropolis transition input objects
        /// </summary>
        [XmlArray("MetropolisTransitions")]
        [XmlArrayItem("MetropolisTransition")]
        public List<XmlMetropolisTransition> MetropolisTransitions { get; set; }

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