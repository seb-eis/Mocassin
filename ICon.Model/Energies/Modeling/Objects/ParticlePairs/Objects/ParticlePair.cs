using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ICon.Model.Particles;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Abstract base class for particle pair implementations taht describe a specific pair interaction occupation
    /// </summary>
    [DataContract]
    public abstract class ParticlePair : IEquatable<ParticlePair>
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
        /// Cchek for equality to other abstract particle pair
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool Equals(ParticlePair other);

        /// <summary>
        /// Generates the hash code for the particle pair
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (1 << Particle0.Index) + (1 << Particle1.Index);
        }
    }
}
