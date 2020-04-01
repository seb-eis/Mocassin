using System.Collections.ObjectModel;
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
        private ModelObjectReference<CellSite> cellReferencePosition = new ModelObjectReference<CellSite>();
        private ObservableCollection<RadialInteractionFilterData> interactionFilters = new ObservableCollection<RadialInteractionFilterData>();
        private double maxInteractionRange;

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
        public ModelObjectReference<CellSite> CellReferencePosition
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
                CellSite = new CellSite {Key = CellReferencePosition.Key},
                InteractionFilters = InteractionFilters.Select(x => x.AsAsymmetric()).ToList()
            };
            return obj;
        }
    }
}