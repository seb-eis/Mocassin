using System;
using System.Xml.Serialization;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.StructureModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Structures.IPositionDummy" /> model object creation
    /// </summary>
    [XmlRoot("PositonDummy")]
    public class DummyPositionGraph : ModelObjectGraph
    {
        private double a;
        private double b;
        private double c;

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

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new PositionDummy
            {
                Vector = new DataVector3D(Math.Abs(A), Math.Abs(B), Math.Abs(C))
            };
            return obj;
        }
    }
}