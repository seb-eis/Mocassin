using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Optimization;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     A simulation job collection for multiple kinetic monte carlo simulation jobs as a single package
    /// </summary>
    public class KmcJobCollection : IJobCollection
    {
        /// <summary>
        ///     Get or set the kinetic simulation the job is valid for
        /// </summary>
        public IKineticSimulation Simulation { get; set; }

        /// <summary>
        ///     Get or set the list of job configurations
        /// </summary>
        public IList<KmcJobConfiguration> JobConfigurations { get; set; }

        /// <summary>
        /// Get or set the list of used <see cref="IPostBuildOptimizer"/>
        /// </summary>
        public IList<IPostBuildOptimizer> PostBuildOptimizers { get; set; }

        /// <inheritdoc />
        public ISimulation GetSimulation()
        {
            return Simulation;
        }

        /// <inheritdoc />
        public IEnumerable<IPostBuildOptimizer> GetPostBuildOptimizers()
        {
            return PostBuildOptimizers.AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<JobConfiguration> GetJobConfigurations()
        {
            return JobConfigurations.AsEnumerable();
        }
    }
}