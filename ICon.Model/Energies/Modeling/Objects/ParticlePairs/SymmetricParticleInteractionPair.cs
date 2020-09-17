using System;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents an symmetric pair of particles to identify pair interactions where the order is not relevant
    /// </summary>
    public class SymmetricParticleInteractionPair : ParticleInteractionPair, IEquatable<SymmetricParticleInteractionPair>
    {
        /// <inheritdoc />
        public bool Equals(SymmetricParticleInteractionPair other)
        {
            if (other == null) return false;
            if (Particle0.Index != other.Particle0.Index)
                return Particle0.Index == other.Particle1.Index && Particle1.Index == other.Particle0.Index;

            return Particle1.Index == other.Particle1.Index;
        }

        /// <inheritdoc />
        public override bool Equals(ParticleInteractionPair other)
        {
            if (other is SymmetricParticleInteractionPair pair) return Equals(pair);
            return false;
        }
    }
}