using System;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents a polar pair of particles to identify pair interactions where the order of the particles is relevant
    /// </summary>
    public class AsymmetricParticleInteractionPair : ParticleInteractionPair, IEquatable<AsymmetricParticleInteractionPair>
    {
        /// <inheritdoc />
        public bool Equals(AsymmetricParticleInteractionPair other) =>
            other != null && Particle0.Index == other.Particle0.Index && Particle1.Index == other.Particle1.Index;

        /// <inheritdoc />
        public override bool Equals(ParticleInteractionPair other)
        {
            if (other is AsymmetricParticleInteractionPair pair) return Equals(pair);

            return false;
        }
    }
}