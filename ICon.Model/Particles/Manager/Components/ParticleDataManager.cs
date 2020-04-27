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
        public int ParticleCount => Data.Particles?.Count ?? 0;

        /// <inheritdoc />
        public int ParticleSetCount => Data.ParticleSets?.Count ?? 0;

        /// <inheritdoc />
        public ParticleDataManager(ParticleModelData modelData)
            : base(modelData)
        {
        }

        /// <inheritdoc />
        public ListReadOnlyWrapper<IParticle> GetParticles()
        {
            return ListReadOnlyWrapper<IParticle>.FromEnumerable(Data.Particles);
        }

        /// <inheritdoc />
        public ListReadOnlyWrapper<IParticleSet> GetParticleSets()
        {
            return ListReadOnlyWrapper<IParticleSet>.FromEnumerable(Data.ParticleSets);
        }

        /// <inheritdoc />
        public int GetValidParticleCount()
        {
            return Data.Particles.Sum(item => item.IsDeprecated ? 0 : 1);
        }

        /// <inheritdoc />
        public int GetValidParticleSetCount()
        {
            return Data.ParticleSets.Sum(item => item.IsDeprecated ? 0 : 1);
        }

        /// <inheritdoc />
        public IParticleSet GetValidParticlesAsSet()
        {
            return new ParticleSet {Particles = Data.Particles.Cast<IParticle>().ToList(), Index = -1};
        }

        /// <inheritdoc />
        public IParticle GetParticle(int index)
        {
            return Data.Particles[index];
        }

        /// <inheritdoc />
        public IParticleSet GetParticleSet(int index)
        {
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