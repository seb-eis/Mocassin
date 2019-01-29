using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.UI.Xml.BaseData
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Mathematics.ValueTypes.IVector3D" /> data
    /// </summary>
    [XmlRoot("D3Vector")]
    public class XmlVector3D
    {
        /// <summary>
        ///     Get or set the A coordinate
        /// </summary>
        [XmlAttribute("A")]
        public double A { get; set; }

        /// <summary>
        ///     Get or set the B coordinate
        /// </summary>
        [XmlAttribute("B")]
        public double B { get; set; }

        /// <summary>
        ///     Get or set the C coordinate
        /// </summary>
        [XmlAttribute("C")]
        public double C { get; set; }

        /// <summary>
        ///     Returns the helper object content as a <see cref="Mocassin.Mathematics.ValueTypes.DataVector3D" /> object
        /// </summary>
        /// <returns></returns>
        public DataVector3D AsDataVector3D()
        {
            return new DataVector3D(A, B, C);
        }

        /// <summary>
        /// Creates a new serializable <see cref="XmlVector3D"/> from a passed <see cref="IVector3D"/> interface
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static XmlVector3D Create(IVector3D vector)
        {
            return new XmlVector3D {A = vector.Coordinates.A, B = vector.Coordinates.B, C = vector.Coordinates.C};
        }
    }
}