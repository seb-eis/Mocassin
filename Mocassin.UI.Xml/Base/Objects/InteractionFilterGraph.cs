using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.Structures;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Model.Energies.IInteractionFilter" /> data
    ///     objects
    /// </summary>
    [XmlRoot("InteractionFilter")]
    public class InteractionFilterGraph : ProjectObjectGraph
    {
        private double startRadius;
        private double endRadius;
        private string centerUnitCellPositionKey;
        private string partnerUnitCellPositionKey;

        /// <summary>
        ///     Get or set the start radius of the filter
        /// </summary>
        [XmlAttribute("StartRadius")]
        [JsonProperty("StartRadius")]
        public double StartRadius
        {
            get => startRadius;
            set => SetProperty(ref startRadius, value);
        }

        /// <summary>
        ///     Get or set the end radius of the filter
        /// </summary>
        [XmlAttribute("EndRadius")]
        [JsonProperty("EndRadius")]
        public double EndRadius
        {
            get => endRadius;
            set => SetProperty(ref endRadius, value);
        }

        /// <summary>
        ///     Get or set the center unit cell position key
        /// </summary>
        [XmlAttribute("FromWyckoff")]
        [JsonProperty("FromWyckoff")]
        public string CenterUnitCellPositionKey
        {
            get => centerUnitCellPositionKey;
            set => SetProperty(ref centerUnitCellPositionKey, value);
        }

        /// <summary>
        ///     Get or set the partner unit cell position key
        /// </summary>
        [XmlAttribute("ToWyckoff")]
        [JsonProperty("ToWyckoff")]
        public string PartnerUnitCellPositionKey
        {
            get => partnerUnitCellPositionKey;
            set => SetProperty(ref partnerUnitCellPositionKey, value);
        }

        /// <summary>
        ///     Creates a symmetric interaction filter from the set information
        /// </summary>
        /// <returns></returns>
        public SymmetricInteractionFilter AsSymmetric()
        {
            var obj = new SymmetricInteractionFilter
            {
                StartRadius = StartRadius,
                EndRadius = EndRadius,
                CenterUnitCellPosition = new UnitCellPosition {Key = CenterUnitCellPositionKey},
                PartnerUnitCellPosition = new UnitCellPosition {Key = PartnerUnitCellPositionKey}
            };
            return obj;
        }

        /// <summary>
        ///     Creates an asymmetric interaction filter from the set information
        /// </summary>
        /// <returns></returns>
        public AsymmetricInteractionFilter AsAsymmetric()
        {
            var obj = new AsymmetricInteractionFilter
            {
                StartRadius = StartRadius,
                EndRadius = EndRadius,
                PartnerUnitCellPosition = new UnitCellPosition {Key = PartnerUnitCellPositionKey}
            };
            return obj;
        }
    }
}