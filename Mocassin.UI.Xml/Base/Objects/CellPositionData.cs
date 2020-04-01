using System.Xml.Serialization;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Serializable helper object for serialization of cell position information
    /// </summary>
    public class CellPositionData : ProjectDataObject
    {
        private ModelObjectReference<Particle> particle;
        private ModelObjectReference<CellSite> referencePosition;
        private VectorData3D vector;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReference{T}" /> to the affiliated <see cref="CellSite" />
        /// </summary>
        [XmlElement("ReferencePosition")]
        public ModelObjectReference<CellSite> ReferencePosition
        {
            get => referencePosition;
            set => SetProperty(ref referencePosition, value);
        }

        /// <summary>
        ///     Get or set the affiliated <see cref="VectorData3D" /> that describes the fractional position
        /// </summary>
        [XmlElement("Vector")]
        public VectorData3D Vector
        {
            get => vector;
            set => SetProperty(ref vector, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReference{T}" /> to the occupation
        ///     <see cref="Mocassin.Model.Particles.Particle" />
        /// </summary>
        [XmlElement("Particle")]
        public ModelObjectReference<Particle> Particle
        {
            get => particle;
            set => SetProperty(ref particle, value);
        }
    }
}