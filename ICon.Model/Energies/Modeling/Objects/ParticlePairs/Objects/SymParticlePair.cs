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
    public class SymParticlePair : IEquatable<SymParticlePair>
    {
        /// <summary>
        /// The first particle interface
        /// </summary>
        [DataMember]
        public IParticle Particle0 { get; set; }

        /// <summary>
        /// The second particle interface
        /// </summary>
        [DataMember]
        public IParticle Particle1 { get; set; }

        /// <summary>
        /// Compares for equality to other unpolar pair
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SymParticlePair other)
        {
            if (Particle0.Index != other.Particle0.Index)
            {
                return Particle0.Index == other.Particle1.Index && Particle1.Index == other.Particle0.Index;
            }
            return Particle1.Index == other.Particle1.Index;
        }

        /// <summary>
        /// Generates the hash code for the unpolar pair
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (1 << Particle0.Index) + (1 << Particle1.Index);
        }
    }
}
