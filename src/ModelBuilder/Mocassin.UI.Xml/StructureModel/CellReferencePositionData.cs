﻿using System;
using System.Xml.Serialization;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.StructureModel
{
    /// <summary>
    ///     Serializable data object for <see cref="ICellSite" /> model object creation
    /// </summary>
    [XmlRoot]
    public class CellReferencePositionData : ModelDataObject
    {
        private double a;
        private double b;
        private double c;
        private ModelObjectReference<ParticleSet> occupation = new ModelObjectReference<ParticleSet>();
        private PositionStability stability = PositionStability.Stable;

        /// <summary>
        ///     Get or set the coordinate value in A direction in [Ang]
        /// </summary>
        [XmlAttribute]
        public double A
        {
            get => a;
            set => SetProperty(ref a, value);
        }

        /// <summary>
        ///     Get or set the coordinate value in B direction in [Ang]
        /// </summary>
        [XmlAttribute]
        public double B
        {
            get => b;
            set => SetProperty(ref b, value);
        }

        /// <summary>
        ///     Get or set the coordinate value in C direction in [Ang]
        /// </summary>
        [XmlAttribute]
        public double C
        {
            get => c;
            set => SetProperty(ref c, value);
        }

        /// <summary>
        ///     Get or set the occupation set reference key
        /// </summary>
        [XmlElement]
        public ModelObjectReference<ParticleSet> Occupation
        {
            get => occupation;
            set => SetProperty(ref occupation, value);
        }

        /// <summary>
        ///     Get or set the position stability status
        /// </summary>
        [XmlAttribute]
        public PositionStability Stability
        {
            get => stability;
            set => SetProperty(ref stability, value);
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new CellSite
            {
                OccupationSet = new ParticleSet {Key = Occupation.Key},
                Stability = Stability,
                Vector = new Fractional3D(A, B, C).TrimToUnitCell(1e-13)
            };
            return obj;
        }
    }
}