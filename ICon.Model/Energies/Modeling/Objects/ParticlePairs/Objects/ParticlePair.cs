using System;
using System.Runtime.Serialization;
using ICon.Model.Particles;

namespace ICon.Model.Energies
{
    /// <summary>
    ///     Abstract base class for particle pair implementations that describe a specific pair interaction occupation.
    /// </summary>
    [DataContract]
    public abstract class ParticlePair : IEquatable<ParticlePair>
    {
        /// <summary>
        ///     The first particle interface
        /// </summary>
        [DataMember]
        public IParticle Particle0 { get; set; }

        /// <summary>
        ///     The second particle interface
        /// </summary>
        [DataMember]
        public IParticle Particle1 { get; set; }

        /// <inheritdoc />
        public abstract bool Equals(ParticlePair other);

        /// <summary>
        ///     Generates the hash code for the particle pair
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (1 << Particle0.Index) + (1 << Particle1.Index);
        }

        /// <summary>
        ///     Creates a new particle pair from center and interaction particle and the information if it should behave asymmetric
        /// </summary>
        /// <param name="center"></param>
        /// <param name="other"></param>
        /// <param name="isAsymmetric"></param>
        /// <returns></returns>
        public static ParticlePair MakePair(IParticle center, IParticle other, bool isAsymmetric)
        {
            if (isAsymmetric) 
                return new AsymmetricParticlePair {Particle0 = center, Particle1 = other};

            return new SymmetricParticlePair {Particle0 = center, Particle1 = other};
        }
    }
}