using System.Collections.Generic;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Represents an access port to the simulation manager data cache that handles supply of cache-able on-demand data
    /// </summary>
    public interface ISimulationCachePort : IModelCachePort
    {
        /// <summary>
        ///     Get a list of the mobile particle infos for all kinetic simulations on all unit cell positions
        /// </summary>
        /// <returns></returns>
        IList<IList<IParticleSet>> GetPositionBoundKineticMobileParticles();

        /// <summary>
        ///     Get a list of the mobile particle infos for all kinetic simulations on all unit cell positions
        /// </summary>
        /// <returns></returns>
        IList<IList<IParticleSet>> GetPositionBoundMetropolisMobileParticles();
    }
}