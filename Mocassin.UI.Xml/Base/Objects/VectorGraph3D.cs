using System.Xml.Serialization;
using Mocassin.Mathematics.ValueTypes;
using Newtonsoft.Json;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Mathematics.ValueTypes.IVector3D" /> data
    /// </summary>
    [XmlRoot("Vector")]
    public class VectorGraph3D : ProjectObjectGraph, IDuplicable<VectorGraph3D>
    {
        private double a;
        private double b;
        private double c;

        /// <summary>
        ///     Get or set the A coordinate
        /// </summary>
        [XmlAttribute("A")]
        [JsonProperty("A")]
        public double A
        {
            get => a;
            set => SetProperty(ref a, value);
        }

        /// <summary>
        ///     Get or set the B coordinate
        /// </summary>
        [XmlAttribute("B")]
        [JsonProperty("B")]
        public double B
        {
            get => b;
            set => SetProperty(ref b, value);
        }

        /// <summary>
        ///     Get or set the C coordinate
        /// </summary>
        [XmlAttribute("C")]
        [JsonProperty("C")]
        public double C
        {
            get => c;
            set => SetProperty(ref c, value);
        }

        /// <summary>
        ///     Returns the helper object content as a <see cref="Mocassin.Mathematics.ValueTypes.DataVector3D" /> object
        /// </summary>
        /// <returns></returns>
        public DataVector3D AsDataVector3D()
        {
            return new DataVector3D(A, B, C);
        }

        /// <summary>
        ///     Creates a new serializable <see cref="VectorGraph3D" /> from a passed <see cref="IVector3D" /> interface
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static VectorGraph3D Create(IVector3D vector)
        {
            return new VectorGraph3D {A = vector.Coordinates.A, B = vector.Coordinates.B, C = vector.Coordinates.C};
        }

        /// <inheritdoc />
        public VectorGraph3D Duplicate()
        {
            return new VectorGraph3D {a = a, b = b, c = c};
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({A.ToString(DefaultCultureInfo)}, {B.ToString(DefaultCultureInfo)}, {C.ToString(DefaultCultureInfo)})";
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }
    }
}