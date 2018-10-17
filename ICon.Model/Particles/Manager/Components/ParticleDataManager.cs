using System;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Particles
{
    /// <summary>
    ///     Basic particle read only data manager that provides read only access to the particle manager base data
    /// </summary>
    internal class ParticleDataManager : ModelDataManager<ParticleModelData>, IParticleDataPort
    {
        /// <inheritdoc />
        public ParticleDataManager(ParticleModelData data)
            : base(data)
        {
        }

        /// <inheritdoc />
        public ReadOnlyListAdapter<IParticle> GetParticles()
        {
            return ReadOnlyListAdapter<IParticle>.FromEnumerable(Data.Particles);
        }

        /// <inheritdoc />
        public ReadOnlyListAdapter<IParticleSet> GetParticleSets()
        {
            return ReadOnlyListAdapter<IParticleSet>.FromEnumerable(Data.ParticleSets);
        }

        /// <inheritdoc />
        public int GetValidParticleCount()
        {
            var count = 0;
            foreach (var item in Data.Particles)
                count += item.IsDeprecated ? 0 : 1;

            return count;
        }

        /// <inheritdoc />
        public int GetValidParticleSetCount()
        {
            var count = 0;
            foreach (var item in Data.ParticleSets) 
                count += item.IsDeprecated ? 0 : 1;

            return count;
        }

        /// <inheritdoc />
        public IParticleSet GetValidParticlesAsSet()
        {
            return new ParticleSet {Particles = Data.Particles.Cast<IParticle>().ToList(), Index = -1};
        }

        /// <inheritdoc />
        public IParticle GetParticle(int index)
        {
            if (index >= Data.Particles.Count) 
                throw new ArgumentOutOfRangeException(nameof(index), "Particle index out of range");

            return Data.Particles[index];
        }

        /// <inheritdoc />
        public IParticleSet GetParticleSet(int index)
        {
            if (index >= Data.ParticleSets.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Particle set index out of range");

            return Data.ParticleSets[index];
        }

        /// <inheritdoc />
        public ReindexingList GetCleanParticleIndexing()
        {
            return CreateReindexing(Data.Particles, Data.Particles.Count);
        }

        /// <inheritdoc />
        public ReindexingList GetCleanParticleSetIndexing()
        {
            return CreateReindexing(Data.ParticleSets, Data.ParticleSets.Count);
        }
    }
}