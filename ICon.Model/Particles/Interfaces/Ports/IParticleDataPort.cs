using Mocassin.Framework.Collections;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Particles
{
    /// <summary>
    ///     Represents a read only data port for the particle manager that allows data access on the particle manager base data
    ///     through interfaces
    /// </summary>
    public interface IParticleDataPort : IModelDataPort
    {
        /// <summary>
        ///     Get the number of defined particles (valid and deprecated) in the data object
        /// </summary>
        int ParticleCount { get; }

        /// <summary>
        ///     Get the number of defined particle sets (valid and deprecated) in the data object
        /// </summary>
        int ParticleSetCount { get; }

        /// <summary>
        ///     Get the current number of valid particles (Deprecated ones are excluded)
        /// </summary>
        /// <returns></returns>
        int GetValidParticleCount();

        /// <summary>
        ///     Get a particle interface by index, also returns deprecated particles
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IParticle GetParticle(int index);

        /// <summary>
        ///     Get a particle set interface by index, also returns deprecated particle sets
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IParticleSet GetParticleSet(int index);

        /// <summary>
        ///     Get the current number of valid particle sets (Deprecated ones are excluded)
        /// </summary>
        /// <returns></returns>
        int GetValidParticleSetCount();

        /// <summary>
        ///     Get a particle set interface with the index (-1) that contains all valid particles
        /// </summary>
        /// <returns></returns>
        IParticleSet GetValidParticlesAsSet();

        /// <summary>
        ///     Get a read only collection of the particle interfaces stored in the manager
        /// </summary>
        ListReadOnlyWrapper<IParticle> GetParticles();

        /// <summary>
        ///     Get a read only collection of the particle set interfaces stored in the manager
        /// </summary>
        ListReadOnlyWrapper<IParticleSet> GetParticleSets();

        /// <summary>
        ///     Get a clean indexing information for the particles that is equivalent to the reindexing info after a deprecated
        ///     data cleanup
        /// </summary>
        /// <returns></returns>
        ReindexingList GetCleanParticleIndexing();

        /// <summary>
        ///     Get a clean indexing information for the particle sets that is equivalent to the reindexing info after a deprecated
        ///     data cleanup
        /// </summary>
        /// <returns></returns>
        ReindexingList GetCleanParticleSetIndexing();
    }
}