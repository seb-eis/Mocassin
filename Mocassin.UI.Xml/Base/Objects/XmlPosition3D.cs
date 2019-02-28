using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Mathematics.ValueTypes.IVector3D" /> data with
    ///     additional position information
    /// </summary>
    [XmlRoot("Position")]
    public class XmlPosition3D : XmlEntity
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
        ///     Get or set an info string for the position
        /// </summary>
        [XmlAttribute("Info")]
        public string Info { get; set; }

        /// <summary>
        ///     Returns the helper object content as a <see cref="Mocassin.Mathematics.ValueTypes.DataVector3D" /> object
        /// </summary>
        /// <returns></returns>
        public DataVector3D AsDataVector3D()
        {
            return new DataVector3D(A, B, C);
        }

        /// <summary>
        ///     Creates a new serializable <see cref="XmlPosition3D" /> from a passed <see cref="IVector3D" /> interface
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static XmlPosition3D Create(IVector3D vector, string info)
        {
            return new XmlPosition3D {A = vector.Coordinates.A, B = vector.Coordinates.B, C = vector.Coordinates.C, Info = info};
        }
    }
}