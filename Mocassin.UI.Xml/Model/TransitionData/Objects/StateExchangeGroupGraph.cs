using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<ModelObjectReferenceGraph<StateExchangePair>> stateExchangePairs;

        /// <summary>
        ///     Get or set the list of state exchange pairs contained in the exchange group
        /// </summary>
        [XmlElement("StateChange")]
        public ObservableCollection<ModelObjectReferenceGraph<StateExchangePair>> StateExchangePairs
        {
            get => stateExchangePairs;
            set => SetProperty(ref stateExchangePairs, value);
        }

        /// <summary>
        ///     Creates new <see cref="StateExchangeGroupGraph" /> with empty component lists
        /// </summary>
        public StateExchangeGroupGraph()
        {
            StateExchangePairs = new ObservableCollection<ModelObjectReferenceGraph<StateExchangePair>>();
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