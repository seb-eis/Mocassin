using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.TransitionModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Transitions.IStateExchangeGroup" /> model object creation
    /// </summary>
    [XmlRoot("StateChangeGroup")]
    public class StateExchangeGroupGraph : ModelObjectGraph
    {
        /// <summary>
        ///     Get or set the list of state exchange pairs contained in the exchange group
        /// </summary>
        [XmlElement("StateChange")]
        public List<ModelObjectReferenceGraph<StateExchangePair>> StateExchangePairs { get; set; }

        /// <summary>
        ///     Creates new <see cref="StateExchangeGroupGraph"/> with empty component lists
        /// </summary>
        public StateExchangeGroupGraph()
        {
            StateExchangePairs = new List<ModelObjectReferenceGraph<StateExchangePair>>();
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new StateExchangeGroup
            {
                StateExchangePairs = StateExchangePairs.Select(x => x.GetInputObject()).Cast<IStateExchangePair>().ToList()
            };
            return obj;
        }
    }
}