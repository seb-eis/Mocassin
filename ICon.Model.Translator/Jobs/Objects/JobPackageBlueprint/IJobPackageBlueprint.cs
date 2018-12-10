using System.Collections.Generic;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     Represents a job package that contains the settings for the simulation database creation of a single simulation
    ///     package
    /// </summary>
    public interface IJobPackageBlueprint : IEnumerable<IJobBlueprint>
    {
        /// <summary>
        ///     Get or set the base simulation model object the package is valid for
        /// </summary>
        ISimulation BaseSimulation { get; set; }

        /// <summary>
        ///     Get or set the list of base job properties that the package uses
        /// </summary>
        IList<IJobProperty> BaseJobProperties { get; set; }

        /// <summary>
        /// Get or set the list of base header job properties that the package uses
        /// </summary>
        IList<IJobProperty> BaseHeaderJobProperties { get; set; }

        /// <summary>
        ///     Add a job blueprint to the job package
        /// </summary>
        /// <param name="jobBlueprint"></param>
        void Add(IJobBlueprint jobBlueprint);
    }
}