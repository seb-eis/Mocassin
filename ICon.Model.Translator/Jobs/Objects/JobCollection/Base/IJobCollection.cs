using System.Collections.Generic;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Optimization;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     Represents a job collection that bundles multiple job configurations for simulation database creation
    /// </summary>
    public interface IJobCollection : IEnumerable<JobConfiguration>
    {
        /// <summary>
        ///     Get the simulation model object the collection is valid for
        /// </summary>
        /// <returns></returns>
        ISimulation GetSimulation();

        /// <summary>
        /// Get the set of defined <see cref="IPostBuildOptimizer"/> for the translation operation
        /// </summary>
        IEnumerable<IPostBuildOptimizer> GetPostBuildOptimizers();
    }
}