using System.Collections.Generic;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     Represents a job collection that bundles multiple job configurations for simulation database creation
    /// </summary>
    public interface IJobCollection
    {
        /// <summary>
        ///     Get the simulation model object the collection is valid for
        /// </summary>
        /// <returns></returns>
        ISimulation GetSimulation();

        /// <summary>
        ///     Get the sequence of job configurations on th collection
        /// </summary>
        /// <returns></returns>
        IEnumerable<JobConfiguration> GetJobConfigurations();
    }
}