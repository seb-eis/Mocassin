using System;
using System.Runtime.Serialization;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents a polar pair of particles to identify pair interactions where the order of the particles is relevant
    /// </summary>
    [DataContract]
    public class AsymmetricParticlePair : ParticlePair, IEquatable<AsymmetricParticlePair>
    {
        /// <inheritdoc />
        public bool Equals(AsymmetricParticlePair other)
        {
            return other != null && (Particle0.Index == other.Particle0.Index && Particle1.Index == other.Particle1.Index);
        }

        /// <inheritdoc />
        public override bool Equals(ParticlePair other)
        {
            if (other is AsymmetricParticlePair pair) 
                return Equals(pair);

            return false;
        }
    }
}