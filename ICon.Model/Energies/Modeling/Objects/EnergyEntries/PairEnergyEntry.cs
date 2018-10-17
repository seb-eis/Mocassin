using System;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Pair energy entry that carries a particle pair and an affiliated energy value
    /// </summary>
    public readonly struct PairEnergyEntry : IEquatable<PairEnergyEntry>
    {
        /// <summary>
        ///     The particle pair that identifies the energy entry
        /// </summary>
        public ParticlePair ParticlePair { get; }

        /// <summary>
        ///     The energy value of the pair entry
        /// </summary>
        public double Energy { get; }

        /// <summary>
        ///     Creates new pair energy entry from particle pair and energy value
        /// </summary>
        /// <param name="particlePair"></param>
        /// <param name="energyValue"></param>
        public PairEnergyEntry(ParticlePair particlePair, double energyValue)
        {
            ParticlePair = particlePair;
            Energy = energyValue;
        }

        /// <summary>
        ///     Get the hash code of the pair energy entry
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ParticlePair.GetHashCode();
        }

        /// <inheritdoc />
        public bool Equals(PairEnergyEntry other)
        {
            return ParticlePair.Equals(other.ParticlePair);
        }

        /// <summary>
        ///     Creates new pair energy entry with the same particle pair but a new energy value
        /// </summary>
        /// <param name="energyValue"></param>
        /// <returns></returns>
        public PairEnergyEntry ChangeEnergy(double energyValue)
        {
            return new PairEnergyEntry(ParticlePair, energyValue);
        }
    }
}