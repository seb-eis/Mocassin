using System;
using System.Xml.Serialization;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.StructureModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Structures.IUnitCellPosition" /> model object creation
    /// </summary>
    [XmlRoot("UnitCellPosition")]
    public class UnitCellPositionGraph : ModelObjectGraph
    {
        private double a;
        private double b;
        private double c;
        private string occupationKey;
        private PositionStatus positionStatus;

        /// <summary>
        ///     Get or set the coordinate value in A direction in [Ang]
        /// </summary>
        [XmlAttribute("A")]
        public double A
        {
            get => a;
            set => SetProperty(ref a, value);
        }

        /// <summary>
        ///     Get or set the coordinate value in B direction in [Ang]
        /// </summary>
        [XmlAttribute("B")]
        public double B
        {
            get => b;
            set => SetProperty(ref b, value);
        }

        /// <summary>
        ///     Get or set the coordinate value in C direction in [Ang]
        /// </summary>
        [XmlAttribute("C")]
        public double C
        {
            get => c;
            set => SetProperty(ref c, value);
        }

        /// <summary>
        ///     Get or set the occupation set reference key
        /// </summary>
        [XmlAttribute("Occupation")]
        public string OccupationKey
        {
            get => occupationKey;
            set => SetProperty(ref occupationKey, value);
        }

        /// <summary>
        ///     Get or set the position stability status
        /// </summary>
        [XmlAttribute("Stability")]
        public PositionStatus PositionStatus
        {
            get => positionStatus;
            set => SetProperty(ref positionStatus, value);
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
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