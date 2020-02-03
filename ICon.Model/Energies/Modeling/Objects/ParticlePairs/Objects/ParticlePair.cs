using System;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Abstract base class for particle pair implementations that describe a specific pair interaction occupation.
    /// </summary>
    public abstract class ParticlePair : IEquatable<ParticlePair>
    {
        /// <summary>
        ///     The first particle interface
        /// </summary>
        [UseTrackedData]
        public IParticle Particle0 { get; set; }

        /// <summary>
        ///     The second particle interface
        /// </summary>
        [UseTrackedData]
        public IParticle Particle1 { get; set; }

        /// <inheritdoc />
        public abstract bool Equals(ParticlePair other);

        /// <summary>
        ///     Generates the hash code for the particle pair
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Particle0.Index + Particle1.Index;
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
            if (isAsymmetric) return new AsymmetricParticlePair {Particle0 = center, Particle1 = other};
            return new SymmetricParticlePair {Particle0 = center, Particle1 = other};
        }
    }
}