using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Collections;
using ICon.Model.Basic;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Basic particle read only data manager that provides read only access to the particle manager base data
    /// </summary>
    internal class ParticleDataManager : ModelDataManager<ParticleModelData>, IParticleDataPort
    {
        /// <summary>
        /// Creates new particle data port wrapper for a data object
        /// </summary>
        /// <param name="data"></param>
        public ParticleDataManager(ParticleModelData data) : base(data)
        {
        }

        /// <summary>
        /// Get all particles as a read ony list of particle interfaces
        /// </summary>
        /// <returns></returns>
        public ReadOnlyListAdapter<IParticle> GetParticles()
        {
            return ReadOnlyListAdapter<IParticle>.FromEnumerable(Data.Particles);
        }

        /// <summary>
        /// Get all particle sets a a read only list of particle set interfaces
        /// </summary>
        /// <returns></returns>
        public ReadOnlyListAdapter<IParticleSet> GetParticleSets()
        {
            return ReadOnlyListAdapter<IParticleSet>.FromEnumerable(Data.ParticleSets);
        }

        /// <summary>
        /// Get the current number of valid particles excluding deprecated ones
        /// </summary>
        /// <returns></returns>
        public int GetValidParticleCount()
        {
            int count = 0;
            foreach (var item in Data.Particles)
            {
                count += (item.IsDeprecated) ? 0 : 1;
            }
            return count;
        }

        /// <summary>
        /// Get the current number of valid particle sets excluding deprecated ones
        /// </summary>
        /// <returns></returns>
        public int GetValidParticleSetCount()
        {
            int count = 0;
            foreach (var item in Data.ParticleSets)
            {
                count += (item.IsDeprecated) ? 0 : 1;
            }
            return count;
        }

        /// <summary>
        /// Returns a particle set that contains all valid particles and has the index -1
        /// </summary>
        /// <returns></returns>
        public IParticleSet GetValidParticlesAsSet()
        {
            return new ParticleSet() { Particles = Data.Particles.Cast<IParticle>().ToList(), Index = -1 };
        }

        /// <summary>
        /// Get a particle interface by index, this function also returns deprecated particles
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IParticle GetParticle(int index)
        {
            if (index >= Data.Particles.Count)
            {
                throw new ArgumentOutOfRangeException("Particle index out of range", nameof(index));
            }
            return Data.Particles[index];
        }

        /// <summary>
        /// Get a particle set interface by index, this funtion also returns deprecated particle sets
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IParticleSet GetParticleSet(int index)
        {
            if (index >= Data.ParticleSets.Count)
            {
                throw new ArgumentOutOfRangeException("Particle set index out of range", nameof(index));
            }
            return Data.ParticleSets[index];
        }

        /// <summary>
        /// Creates a clean particle reindexing information that shows the reindexing if deprecated particles are removed
        /// </summary>
        /// <returns></returns>
        public ReindexingList GetCleanParticleIndexing()
        {
            return CreateReindexing(Data.Particles, Data.Particles.Count);
        }

        /// <summary>
        /// Creates a clean particle set reindexing information that shows the reindexing if deprecated particles are removed
        /// </summary>
        /// <returns></returns>
        public ReindexingList GetCleanParticleSetIndexing()
        {
            return CreateReindexing(Data.ParticleSets, Data.ParticleSets.Count);
        }
    }
}
