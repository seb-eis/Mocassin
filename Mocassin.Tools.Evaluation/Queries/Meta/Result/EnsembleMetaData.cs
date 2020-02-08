using System;
using Mocassin.Model.Particles;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Stores meta information for an ensemble of simulation particles
    /// </summary>
    public readonly struct EnsembleMetaData
    {
        /// <summary>
        ///     Get the <see cref="IParticle" /> that the data belongs to
        /// </summary>
        public IParticle Particle { get; }

        /// <summary>
        ///     Get the number of particles in the ensemble
        /// </summary>
        public int ParticleCount { get; }

        /// <summary>
        ///     Get the density of particles in [1/m^3] for the ensemble
        /// </summary>
        public double ParticleDensity { get; }

        public EnsembleMetaData(IParticle particle, int particleCount, double particleDensity)
            : this()
        {
            Particle = particle ?? throw new ArgumentNullException(nameof(particle));
            ParticleCount = particleCount;
            ParticleDensity = particleDensity;
        }
    }
}