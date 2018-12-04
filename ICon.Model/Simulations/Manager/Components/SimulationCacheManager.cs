using System.Collections.Generic;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Implementation of the simulation cache manager that handles creation and supply of on-demand simulation data
    ///     objects
    /// </summary>
    internal class SimulationCacheManager : ModelCacheManager<SimulationModelCache, ISimulationCachePort>, ISimulationCachePort
    {
        /// <inheritdoc />
        public SimulationCacheManager(SimulationModelCache modelCache, IModelProject modelProject)
            : base(modelCache, modelProject)
        {
        }
    }
}