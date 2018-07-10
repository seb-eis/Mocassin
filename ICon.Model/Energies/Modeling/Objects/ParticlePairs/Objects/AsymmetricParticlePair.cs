using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Model.Particles;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Represents a polar pair of particles to identify pair interactions where the order of the particles is relevant
    /// </summary>
    [DataContract]
    public class AsymmetricParticlePair : ParticlePair, IEquatable<AsymmetricParticlePair>
    {
        /// <summary>
        /// Compares for eqality to other unpolar pair
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AsymmetricParticlePair other)
        {
            return Particle0.Index == other.Particle0.Index && Particle1.Index == other.Particle1.Index;
        }

        /// <summary>
        /// Cechks for equality with other particle pair. Always retruns false if other pair is not asymmetric
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(ParticlePair other)
        {
            if (other is AsymmetricParticlePair pair)
            {
                return Equals(pair);
            }
            return false;
        }
    }
}
