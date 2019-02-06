﻿using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.Structures;

namespace Mocassin.UI.Xml.BaseData
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Model.Energies.IInteractionFilter" /> data
    ///     objects
    /// </summary>
    [XmlRoot("InteractionFilter")]
    public class XmlInteractionFilter
    {
        /// <summary>
        ///     Get or set the start radius of the filter
        /// </summary>
        [XmlAttribute("StartRadius")]
        public double StartRadius { get; set; }

        /// <summary>
        ///     Get or set the end radius of the filter
        /// </summary>
        [XmlAttribute("EndRadius")]
        public double EndRadius { get; set; }

        /// <summary>
        ///     Get or set the center unit cell position key
        /// </summary>
        [XmlAttribute("FromWyckoff")]
        public string CenterUnitCellPositionKey { get; set; }

        /// <summary>
        ///     Get or set the partner unit cell position key
        /// </summary>
        [XmlAttribute("ToWyckoff")]
        public string PartnerUnitCellPositionKey { get; set; }

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