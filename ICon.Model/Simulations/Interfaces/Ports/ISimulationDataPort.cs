using System.Collections.Generic;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Represents an access port to the simulation manager model data that provides read only data access
    /// </summary>
    public interface ISimulationDataPort : IModelDataPort
    {
        /// <summary>
        ///     Get the kinetic simulation at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IKineticSimulation GetKineticSimulation(int index);

        /// <summary>
        ///     Get a read only list of all defined kinetic simulations
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IKineticSimulation> GetKineticSimulations();

        /// <summary>
        ///     Get the metropolis simulation at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IMetropolisSimulation GetMetropolisSimulation(int index);

        /// <summary>
        ///     Get a read only list of all defined metropolis simulations
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IMetropolisSimulation> GetMetropolisSimulations();
    }
}