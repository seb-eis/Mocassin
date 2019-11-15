using System.Collections.Generic;
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
    [XmlRoot("StableEnvironmentInfo")]
    public class StableEnvironmentGraph : ModelParameterGraph
    {
        private double maxInteractionRange;
        private ObservableCollection<InteractionFilterGraph> interactionFilters;
        private DefectBackgroundGraph defectBackground;

        /// <summary>
        ///     Get or set the interaction cutoff range in [Ang]
        /// </summary>
        [XmlAttribute("InteractionRadius")]
        public double MaxInteractionRange
        {
            get => maxInteractionRange;
            set => SetProperty(ref maxInteractionRange, value);
        }

        /// <summary>
        ///     Get or set the list of interaction filters for stable environments
        /// </summary>
        [XmlArray("InteractionFilters")]
        [XmlArrayItem("Filter")]
        public ObservableCollection<InteractionFilterGraph> InteractionFilters
        {
            get => interactionFilters;
            set => SetProperty(ref interactionFilters, value);
        }

        /// <summary>
        ///     Get or set the <see cref="DefectBackgroundGraph" /> for stable environments
        /// </summary>
        [XmlElement("DefectBackground")]
        public DefectBackgroundGraph DefectBackground
        {
            get => defectBackground;
            set => SetProperty(ref defectBackground, value);
        }

        /// <summary>
        ///     Creates new <see cref="StableEnvironmentGraph" /> with empty component lists
        /// </summary>
        public StableEnvironmentGraph()
        {
            InteractionFilters = new ObservableCollection<InteractionFilterGraph>();
            DefectBackground = new DefectBackgroundGraph();
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