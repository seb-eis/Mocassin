﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Energies;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.EnergyModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Energies.IUnstableEnvironment" /> model object creation
    /// </summary>
    [XmlRoot]
    public class UnstableEnvironmentData : ModelDataObject
    {
        private double maxInteractionRange;
        private ModelObjectReference<CellReferencePosition> cellReferencePosition = new ModelObjectReference<CellReferencePosition>();
        private ObservableCollection<RadialInteractionFilterData> interactionFilters = new ObservableCollection<RadialInteractionFilterData>();

        /// <summary>
        ///     Get or set the maximum interaction range
        /// </summary>
        [XmlAttribute]
        public double MaxInteractionRange
        {
            get => maxInteractionRange;
            set => SetProperty(ref maxInteractionRange, value);
        }

        /// <summary>
        ///     Get or set the key of the center unit cell position
        /// </summary>
        [XmlElement]
        public ModelObjectReference<CellReferencePosition> CellReferencePosition
        {
            get => cellReferencePosition;
            set => SetProperty(ref cellReferencePosition, value);
        }

        /// <summary>
        ///     Get or set the list of interaction filters of the environment
        /// </summary>
        [XmlArray]
        public ObservableCollection<RadialInteractionFilterData> InteractionFilters
        {
            get => interactionFilters;
            set => SetProperty(ref interactionFilters, value);
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new UnstableEnvironment
            {
                MaxInteractionRange = MaxInteractionRange,
                CellReferencePosition = new CellReferencePosition {Key = CellReferencePosition.Key},
                InteractionFilters = InteractionFilters.Select(x => x.AsAsymmetric()).ToList()
            };
            return obj;
        }
    }
}