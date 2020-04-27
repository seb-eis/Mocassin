using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Optimization;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     A simulation job collection for multiple metropolis monte carlo simulation jobs as a single package
    /// </summary>
    public class MmcJobCollection : IJobCollection
    {
        /// <summary>
        ///     Get or set the kinetic simulation the job is valid for
        /// </summary>
        public IMetropolisSimulation Simulation { get; set; }

        /// <summary>
        ///     Get or set the list of job configurations
        /// </summary>
        public IList<MmcJobConfiguration> JobConfigurations { get; set; }

        /// <summary>
        ///     Get or set the list of used <see cref="IPostBuildOptimizer" />
        /// </summary>
        public IList<IPostBuildOptimizer> PostBuildOptimizers { get; set; }

        /// <inheritdoc />
        public int CollectionId { get; set; }

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
        public IEnumerator<JobConfiguration> GetEnumerator()
        {
            return JobConfigurations.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}