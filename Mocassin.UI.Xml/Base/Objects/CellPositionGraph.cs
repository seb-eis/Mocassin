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
        private ModelObjectReferenceGraph<Particle> occupation;
        private ModelObjectReferenceGraph<UnitCellPosition> wyckoffPosition;
        private VectorGraph3D vector;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> to the occupation <see cref="Particle" />
        /// </summary>
        [XmlElement("Particle")]
        public ModelObjectReferenceGraph<Particle> Occupation
        {
            get => occupation;
            set => SetProperty(ref occupation, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> to the affiliated <see cref="UnitCellPosition" />
        /// </summary>
        [XmlElement("Wyckoff")]
        public ModelObjectReferenceGraph<UnitCellPosition> WyckoffPosition
        {
            get => wyckoffPosition;
            set => SetProperty(ref wyckoffPosition, value);
        }

        /// <summary>
        ///     Get or set the affiliated <see cref="VectorGraph3D" /> that describes the fractional position
        /// </summary>
        [XmlElement("Vector")]
        public VectorGraph3D Vector
        {
            get => vector;
            set => SetProperty(ref vector, value);
        }
    }
}