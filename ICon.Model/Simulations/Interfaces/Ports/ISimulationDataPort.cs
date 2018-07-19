using System;
using System.Collections.Generic;
using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Represents an access port to the simulation manager model data that provides read only data access
    /// </summary>
    public interface ISimulationDataPort : IModelDataPort
    {
        /// <summary>
        /// Get the kinetic simulation at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IKineticSimulation GetKineticSimulation(int index);

        /// <summary>
        /// Get a read only list of all defined kinetic simulations
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IKineticSimulation> GetKineticSimulations();

        /// <summary>
        /// Get the metropolis simulation at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IMetropolisSimulation GetMetropolisSimulation(int index);

        /// <summary>
        /// Get a read only list of all defined metropolis simulations
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IMetropolisSimulation> GetMetropolisSimulations();

        /// <summary>
        /// Get the kinetic simulation series at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IKineticSimulationSeries GetKineticSeries(int index);

        /// <summary>
        /// Get a read only list of all defined kinetic simulation series
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IKineticSimulationSeries> GetKineticSeriesList();

        /// <summary>
        /// Get the metropolis simulation series at the specififed index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IMetropolisSimulationSeries GetMetropolisSeries(int index);

        /// <summary>
        /// Get a read only list of all defined metropolis simulation series
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IMetropolisSimulationSeries> GetMetropolisSeriesList();
    }
}
