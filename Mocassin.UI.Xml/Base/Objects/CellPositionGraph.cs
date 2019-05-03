using System.Xml.Serialization;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Serializable helper object for serialization of cell position information
    /// </summary>
    [XmlRoot("CellPosition")]
    public class CellPositionGraph : ProjectObjectGraph
    {
        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> to the occupation <see cref="Particle" />
        /// </summary>
        [XmlElement("Particle")]
        public ModelObjectReferenceGraph<Particle> Occupation { get; set; }

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> to the affiliated wyckoff
        ///     <see cref="UnitCellPosition" />
        /// </summary>
        [XmlElement("Wyckoff")]
        public ModelObjectReferenceGraph<UnitCellPosition> WyckoffPosition { get; set; }

        /// <summary>
        ///     Get or set the affiliated <see cref="VectorGraph3D" /> that describes the fractional position
        /// </summary>
        [XmlElement("Vector")]
        public VectorGraph3D Vector { get; set; }
    }
}