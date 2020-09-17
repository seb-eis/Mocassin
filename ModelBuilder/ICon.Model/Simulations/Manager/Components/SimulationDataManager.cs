using System.Collections.Generic;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Simulation data manager implementation that provided read only access to the simulation model data object
    /// </summary>
    public class SimulationDataManager : ModelDataManager<SimulationModelData>, ISimulationDataPort
    {
        /// <inheritdoc />
        public SimulationDataManager(SimulationModelData modelData)
            : base(modelData)
        {
        }

        /// <inheritdoc />
        public IKineticSimulation GetKineticSimulation(int index) => Data.KineticSimulations[index];

        /// <inheritdoc />
        public IReadOnlyList<IKineticSimulation> GetKineticSimulations() => Data.KineticSimulations;

        /// <inheritdoc />
        public IMetropolisSimulation GetMetropolisSimulation(int index) => Data.MetropolisSimulations[index];

        /// <inheritdoc />
        public IReadOnlyList<IMetropolisSimulation> GetMetropolisSimulations() => Data.MetropolisSimulations;
    }
}