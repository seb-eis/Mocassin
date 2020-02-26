using System;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Pair energy entry that carries a particle pair and an affiliated energy value
    /// </summary>
    public class PairEnergyEntry : IEquatable<PairEnergyEntry>
    {
        /// <summary>
        ///     The <see cref="Mocassin.Model.Energies.ParticleInteractionPair"/> that identifies the energy entry
        /// </summary>
        [UseTrackedData(ReferenceCorrectionLevel = ReferenceCorrectionLevel.IgnoreTopLevel)]
        public ParticleInteractionPair ParticleInteractionPair { get; }

        /// <summary>
        ///     The energy value of the pair entry
        /// </summary>
        public double Energy { get; }

        /// <summary>
        ///     Creates new pair energy entry from particle pair and energy value
        /// </summary>
        /// <param name="particleInteractionPair"></param>
        /// <param name="energyValue"></param>
        public PairEnergyEntry(ParticleInteractionPair particleInteractionPair, double energyValue)
        {
            ParticleInteractionPair = particleInteractionPair;
            Energy = energyValue;
        }

        /// <summary>
        ///     Get the hash code of the pair energy entry
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ParticleInteractionPair.GetHashCode();
        }

        /// <inheritdoc />
        public bool Equals(PairEnergyEntry other)
        {
            return other != null && ParticleInteractionPair.Equals(other.ParticleInteractionPair);
        }

        /// <summary>
        ///     Creates new pair energy entry with the same particle pair but a new energy value
        /// </summary>
        /// <param name="energyValue"></param>
        /// <returns></returns>
        public PairEnergyEntry ChangeEnergy(double energyValue)
        {
            return new PairEnergyEntry(ParticleInteractionPair, energyValue);
        }
    }
}