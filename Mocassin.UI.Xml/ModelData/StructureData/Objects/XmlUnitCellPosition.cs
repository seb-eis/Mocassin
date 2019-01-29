using System;
using System.Xml.Serialization;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.StructureData
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Structures.IUnitCellPosition" /> model object creation
    /// </summary>
    [XmlRoot("UnitCellPosition")]
    public class XmlUnitCellPosition : XmlModelObject
    {
        /// <summary>
        ///     Get or set the coordinate value in A direction in [Ang]
        /// </summary>
        [XmlAttribute("A")]
        public double A { get; set; }

        /// <summary>
        ///     Get or set the coordinate value in B direction in [Ang]
        /// </summary>
        [XmlAttribute("B")]
        public double B { get; set; }

        /// <summary>
        ///     Get or set the coordinate value in C direction in [Ang]
        /// </summary>
        [XmlAttribute("C")]
        public double C { get; set; }

        /// <summary>
        ///     Get or set the occupation set reference key
        /// </summary>
        [XmlAttribute("Occupation")]
        public string OccupationKey { get; set; }

        /// <summary>
        ///     Get or set the position stability status
        /// </summary>
        [XmlAttribute("Stability")]
        public PositionStatus PositionStatus { get; set; }

        /// <inheritdoc />
        protected override ModelObject GetPreparedModelObject()
        {
            var obj = new UnitCellPosition
            {
                OccupationSet = new ParticleSet {Key = OccupationKey},
                Status = PositionStatus,
                Vector = new DataVector3D(Math.Abs(A), Math.Abs(B), Math.Abs(C))
            };
            return obj;
        }
    }
}