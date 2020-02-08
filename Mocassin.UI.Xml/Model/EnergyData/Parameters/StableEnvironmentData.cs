using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Energies;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.EnergyModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Energies.IStableEnvironmentInfo" /> model parameter creation
    /// </summary>
    [XmlRoot]
    public class StableEnvironmentData : ModelParameterObject
    {
        private DefectBackgroundData defectBackground;
        private ObservableCollection<RadialInteractionFilterData> interactionFilters;
        private double maxInteractionRange;

        /// <summary>
        ///     Get or set the interaction cutoff range in [Ang]
        /// </summary>
        [XmlAttribute]
        public double MaxInteractionRange
        {
            get => maxInteractionRange;
            set => SetProperty(ref maxInteractionRange, value);
        }

        /// <summary>
        ///     Get or set the list of interaction filters for stable environments
        /// </summary>
        [XmlArray]
        public ObservableCollection<RadialInteractionFilterData> InteractionFilters
        {
            get => interactionFilters;
            set => SetProperty(ref interactionFilters, value);
        }

        /// <summary>
        ///     Get or set the <see cref="DefectBackgroundData" /> for stable environments
        /// </summary>
        [XmlElement]
        public DefectBackgroundData DefectBackground
        {
            get => defectBackground;
            set => SetProperty(ref defectBackground, value);
        }

        /// <summary>
        ///     Creates new <see cref="StableEnvironmentData" /> with empty component lists
        /// </summary>
        public StableEnvironmentData()
        {
            InteractionFilters = new ObservableCollection<RadialInteractionFilterData>();
            DefectBackground = new DefectBackgroundData();
        }

        /// <inheritdoc />
        protected override ModelParameter GetModelObjectInternal()
        {
            var obj = new StableEnvironmentInfo
            {
                MaxInteractionRange = MaxInteractionRange,
                InteractionFilters = InteractionFilters.Select(x => x.AsSymmetric()).ToList(),
                DefectEnergies = DefectBackground.AsDefectList()
            };
            return obj;
        }
    }
}