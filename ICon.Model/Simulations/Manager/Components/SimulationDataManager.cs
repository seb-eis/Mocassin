using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Simulation data manager implementation that provided read only access to the simulation model data object
    /// </summary>
    public class SimulationDataManager : ModelDataManager<SimulationModelData>, ISimulationDataPort
    {
        /// <summary>
        /// Creates a new simulation data manager adapter for the provided model data object
        /// </summary>
        /// <param name="data"></param>
        public SimulationDataManager(SimulationModelData data) : base(data)
        {

        }

        /// <summary>
        /// Get the kinetic simulation at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IKineticSimulation GetKineticSimulation(int index)
        {
            return Data.KineticSimulations[index];
        }

        /// <summary>
        /// Get a read only list of all defined kinetic simulations
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IKineticSimulation> GetKineticSimulations()
        {
            return Data.KineticSimulations;
        }

        /// <summary>
        /// Get the metropolis simulation at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IMetropolisSimulation GetMetropolisSimulation(int index)
        {
            return Data.MetropolisSimulations[index];
        }

        /// <summary>
        /// Get a read only list of all defined metropolis simulations
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IMetropolisSimulation> GetMetropolisSimulations()
        {
            return Data.MetropolisSimulations;
        }

        /// <summary>
        /// Get the kinetic simulation series at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IKineticSimulationSeries GetKineticSeries(int index)
        {
            return Data.KineticSeries[index];
        }

        /// <summary>
        /// Get a read only list of all defined kinetic simulation series
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IKineticSimulationSeries> GetKineticSeriesList()
        {
            return Data.KineticSeries;
        }

        /// <summary>
        /// Get the metropolis simulation series at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IMetropolisSimulationSeries GetMetropolisSeries(int index)
        {
            return Data.MetropolisSeries[index];
        }

        /// <summary>
        /// Get a read only list of all defined metropolis simulation series
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IMetropolisSimulationSeries> GetMetropolisSeriesList()
        {
            return Data.MetropolisSeries;
        }
    }
}
