using Mocassin.Model.Basic;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Implementation of the simulation query manager that provides save query based access to the simulation model data
    ///     and cache
    /// </summary>
    internal class SimulationQueryManager :
        ModelQueryManager<SimulationModelData, ISimulationDataPort, SimulationModelCache, ISimulationCachePort>, ISimulationQueryPort
    {
        /// <inheritdoc />
        public SimulationQueryManager(SimulationModelData modelData, SimulationModelCache modelCacheModel, AccessLockSource lockSource)
            : base(modelData, modelCacheModel, lockSource)
        {
        }
    }
}