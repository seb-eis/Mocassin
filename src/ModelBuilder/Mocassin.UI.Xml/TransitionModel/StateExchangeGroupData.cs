﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Transitions;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.TransitionModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Transitions.IStateExchangeGroup" /> model object creation
    /// </summary>
    [XmlRoot]
    public class StateExchangeGroupData : ModelDataObject
    {
        private ObservableCollection<ModelObjectReference<StateExchangePair>> stateExchangePairs =
            new ObservableCollection<ModelObjectReference<StateExchangePair>>();

        /// <summary>
        ///     Get or set the list of state exchange pairs contained in the exchange group
        /// </summary>
        [XmlElement]
        public ObservableCollection<ModelObjectReference<StateExchangePair>> StateExchangePairs
        {
            get => stateExchangePairs;
            set => SetProperty(ref stateExchangePairs, value);
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