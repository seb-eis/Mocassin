using System.Xml.Serialization;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     A position info that bundles 3D coordinate information with unit cell position information for easy identification
    ///     by a user
    /// </summary>
    [XmlRoot]
    public class MmlPosition
    {
        /// <summary>
        ///     Get or set the first coordinate value
        /// </summary>
        [XmlAttribute("A")]
        public double A { get; set; }

        /// <summary>
        ///     Get or set the first coordinate value
        /// </summary>
        [XmlAttribute("B")]
        public double B { get; set; }

        /// <summary>
        ///     Get or set the first coordinate value
        /// </summary>
        [XmlAttribute("C")]
        public double C { get; set; }

        /// <summary>
        ///     Get or set the unit cell position index
        /// </summary>
        [XmlAttribute("UcpId")]
        public int UnitCellPositionId { get; set; }

        /// <summary>
        ///     Get or set the unit cell position alias
        /// </summary>
        [XmlAttribute("UcpAlias")]
        public string UnitCellPositionAlias { get; set; }


        /// <summary>
        ///     Creates new Mml position from a unit cell position model object
        /// </summary>
        /// <param name="unitCellPosition"></param>
        /// <returns></returns>
        public static MmlPosition FromModelObject(IUnitCellPosition unitCellPosition)
        {
            return new MmlPosition
            {
                UnitCellPositionId = unitCellPosition.Index,
                UnitCellPositionAlias = unitCellPosition.Alias,
                A = unitCellPosition.Vector.A,
                B = unitCellPosition.Vector.B,
                C = unitCellPosition.Vector.C
            };
        }
    }
}