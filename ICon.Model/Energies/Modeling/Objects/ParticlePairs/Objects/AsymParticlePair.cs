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
    public class AsymParticlePair : IEquatable<AsymParticlePair>
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
        /// Compares for eqality to other unpolar pair
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AsymParticlePair other)
        {
            return Particle0.Index == other.Particle0.Index && Particle1.Index == other.Particle1.Index;
        }

        /// <summary>
        /// Generates the hash code for the polar pair
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (1 << Particle0.Index) + (1 << Particle1.Index);
        }
    }
}
