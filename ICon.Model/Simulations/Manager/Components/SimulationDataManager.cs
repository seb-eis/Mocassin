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
        public IKineticSimulation GetKineticSimulation(int index)
        {
            return Data.KineticSimulations[index];
        }

        /// <inheritdoc />
        public IReadOnlyList<IKineticSimulation> GetKineticSimulations()
        {
            return Data.KineticSimulations;
        }

        /// <inheritdoc />
        public IMetropolisSimulation GetMetropolisSimulation(int index)
        {
            return Data.MetropolisSimulations[index];
        }

        /// <inheritdoc />
        public IReadOnlyList<IMetropolisSimulation> GetMetropolisSimulations()
        {
            return Data.MetropolisSimulations;
        }

        /// <inheritdoc />
        public IKineticSimulationSeries GetKineticSeries(int index)
        {
            return Data.KineticSeries[index];
        }

        /// <inheritdoc />
        public IReadOnlyList<IKineticSimulationSeries> GetKineticSeriesList()
        {
            return Data.KineticSeries;
        }

        /// <inheritdoc />
        public IMetropolisSimulationSeries GetMetropolisSeries(int index)
        {
            return Data.MetropolisSeries[index];
        }

        /// <inheritdoc />
        public IReadOnlyList<IMetropolisSimulationSeries> GetMetropolisSeriesList()
        {
            return Data.MetropolisSeries;
        }
    }
}