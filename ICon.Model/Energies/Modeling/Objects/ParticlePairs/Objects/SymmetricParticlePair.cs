using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Model.Particles;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Represents an unpolar pair of particles to identify pair interactions where the order is not relevant
    /// </summary>
    [DataContract]
    public class SymmetricParticlePair : ParticlePair, IEquatable<SymmetricParticlePair>
    {
        /// <summary>
        /// Compares for equality to other unpolar pair
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SymmetricParticlePair other)
        {
            if (Particle0.Index != other.Particle0.Index)
            {
                return Particle0.Index == other.Particle1.Index && Particle1.Index == other.Particle0.Index;
            }
            return Particle1.Index == other.Particle1.Index;
        }


        /// <summary>
        /// Cechks for equality with other particle pair. Always retruns false if other pair is not symmetric
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(ParticlePair other)
        {
            if (other is SymmetricParticlePair pair)
            {
                return Equals(pair);
            }
            return false;
        }
    }
}
