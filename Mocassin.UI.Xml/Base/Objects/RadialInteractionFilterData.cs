using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.Structures;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Model.Energies.IInteractionFilter" /> data
    ///     objects
    /// </summary>
    [XmlRoot("RadialInteractionFilter")]
    public class RadialInteractionFilterData : ProjectDataObject
    {
        private ModelObjectReference<CellSite> centerCellReferencePosition;
        private double endRadius;
        private ModelObjectReference<CellSite> partnerCellReferencePosition;
        private double startRadius;

        /// <summary>
        ///     Get or set the start radius of the filter
        /// </summary>
        [XmlAttribute("StartRadius")]
        public double StartRadius
        {
            get => startRadius;
            set => SetProperty(ref startRadius, value);
        }

        /// <summary>
        ///     Get or set the end radius of the filter
        /// </summary>
        [XmlAttribute("EndRadius")]
        public double EndRadius
        {
            get => endRadius;
            set => SetProperty(ref endRadius, value);
        }

        /// <summary>
        ///     Get or set the center unit cell position key
        /// </summary>
        [XmlElement("CenterCellReferencePosition")]
        public ModelObjectReference<CellSite> CenterCellReferencePosition
        {
            get => centerCellReferencePosition;
            set => SetProperty(ref centerCellReferencePosition, value);
        }

        /// <summary>
        ///     Get or set the partner unit cell position key
        /// </summary>
        [XmlElement("PartnerCellReferencePosition")]
        public ModelObjectReference<CellSite> PartnerCellReferencePosition
        {
            get => partnerCellReferencePosition;
            set => SetProperty(ref partnerCellReferencePosition, value);
        }

        /// <summary>
        ///     Creates a symmetric interaction filter from the set information
        /// </summary>
        /// <returns></returns>
        public StableInteractionFilter AsSymmetric()
        {
            var obj = new StableInteractionFilter
            {
                StartRadius = StartRadius,
                EndRadius = EndRadius,
                CenterCellSite = new CellSite {Key = CenterCellReferencePosition.Key},
                PartnerCellSite = new CellSite {Key = PartnerCellReferencePosition.Key}
            };
            return obj;
        }

        /// <summary>
        ///     Creates an asymmetric interaction filter from the set information
        /// </summary>
        /// <returns></returns>
        public UnstableInteractionFilter AsAsymmetric()
        {
            var obj = new UnstableInteractionFilter
            {
                StartRadius = StartRadius,
                EndRadius = EndRadius,
                PartnerCellSite = new CellSite {Key = PartnerCellReferencePosition.Key}
            };
            return obj;
        }
    }
}