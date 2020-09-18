using System;
using System.Diagnostics;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Abstract base class for particle pair implementations that describe a specific pair interaction occupation.
    /// </summary>
    [DebuggerDisplay("{Particle0} {Particle1}")]
    public abstract class ParticleInteractionPair : IEquatable<ParticleInteractionPair>
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
        public abstract bool Equals(ParticleInteractionPair other);

        /// <summary>
        ///     Generates the hash code for the particle pair
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Particle0.Index + Particle1.Index;

        /// <summary>
        ///     Creates a new particle pair from center and interaction <see cref="IParticle" /> with an information if its
        ///     symmetric or not
        /// </summary>
        /// <param name="center"></param>
        /// <param name="other"></param>
        /// <param name="isSymmetric"></param>
        /// <returns></returns>
        public static ParticleInteractionPair MakePair(IParticle center, IParticle other, bool isSymmetric)
        {
            if (!isSymmetric) return new AsymmetricParticleInteractionPair {Particle0 = center, Particle1 = other};
            return new SymmetricParticleInteractionPair {Particle0 = center, Particle1 = other};
        }
    }
}