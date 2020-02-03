using System;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Describes a defect energy of an <see cref="IParticle" /> on a specific <see cref="ICellReferencePosition" />
    /// </summary>
    [DataContract]
    public class DefectEnergy : IDefectEnergy
    {
        /// <inheritdoc />
        [DataMember]
        [UseTrackedReferences]
        public IParticle Particle { get; set; }

        /// <inheritdoc />
        [DataMember]
        [UseTrackedReferences]
        public ICellReferencePosition CellReferencePosition { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double Energy { get; set; }

        /// <inheritdoc />
        public int CompareTo(IDefectEnergy other)
        {
            if (other == null) return -1;
            if (ReferenceEquals(this, other)) return 0;
            var particleCompare = Particle.Index.CompareTo(other.Particle.Index);
            return particleCompare == 0 ? CellReferencePosition.Index.CompareTo(other.CellReferencePosition.Index) : particleCompare;
        }

        /// <inheritdoc />
        public bool Equals(IDefectEnergy other)
        {
            return CompareTo(other) == 0;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 254019327;
            hashCode = hashCode * -1521134295 + Particle.Index;
            hashCode = hashCode * -1521134295 + CellReferencePosition.Index;
            return hashCode;
        }

        /// <summary>
        ///     Creates a new <see cref="DefectEnergy"/> form an <see cref="IDefectEnergy"/> interface
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DefectEnergy FromInterface(IDefectEnergy source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return new DefectEnergy
            {
                Energy = source.Energy,
                Particle = source.Particle,
                CellReferencePosition = source.CellReferencePosition
            };
        }
    }
}