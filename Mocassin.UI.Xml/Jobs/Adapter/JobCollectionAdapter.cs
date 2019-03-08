using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Random;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.Optimization;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Adapter class to provide <see cref="JobPackageDescriptionGraph" /> data as a <see cref="IJobCollection" /> object for the
    ///     database creation system
    /// </summary>
    public class JobCollectionAdapter : IJobCollection
    {
        /// <summary>
        ///     Get the <see cref="IModelProject" /> the source is affiliated with
        /// </summary>
        private IModelProject ModelProject { get; set; }

        /// <summary>
        ///     Get the <see cref="ISimulation" /> object the collection is valid for
        /// </summary>
        private ISimulation Simulation { get; set; }

        /// <summary>
        ///     Get the <see cref="JobPackageDescriptionGraph" /> that is used as a <see cref="JobPackageDescriptionGraph" /> source
        /// </summary>
        private JobPackageDescriptionGraph JobPackageDescription { get; set; }

        /// <summary>
        ///     Get the <see cref="JobConfiguration" /> that provides the default parameters
        /// </summary>
        private JobConfiguration BaseConfiguration { get; set; }

        /// <summary>
        ///     Get or set the random number generator used for the collection
        /// </summary>
        private Random Random { get; set; }

        /// <inheritdoc />
        public ISimulation GetSimulation()
        {
            return Simulation;
        }

        /// <inheritdoc />
        public IEnumerable<IPostBuildOptimizer> GetPostBuildOptimizers()
        {
            return JobPackageDescription.ManualOptimizers.Select(x => x.ToInternal(ModelProject));
        }

        /// <inheritdoc />
        public IEnumerable<JobConfiguration> GetJobConfigurations()
        {
            BaseConfiguration.LatticeConfiguration = new LatticeConfiguration {SizeA = 10, SizeB = 10, SizeC = 10};
            var jobCount = JobPackageDescription.JobCountPerConfig is null ? Simulation.JobCount : int.Parse(JobPackageDescription.JobCountPerConfig);
            return JobPackageDescription.GetConfigurations().SelectMany(x => ExpandToJobCount(x.ToInternal(BaseConfiguration), jobCount));
        }

        /// <summary>
        ///     Expands the passed <see cref="JobConfiguration" /> to the required job count and sets affiliated random information
        /// </summary>
        /// <param name="jobConfiguration"></param>
        /// <param name="jobCount"></param>
        /// <returns></returns>
        private IEnumerable<JobConfiguration> ExpandToJobCount(JobConfiguration jobConfiguration, int jobCount)
        {
            var buffer = new byte[16];
            for (var i = 0; i < jobCount; i++)
            {
                var result = jobConfiguration.DeepCopy();
                Random.NextBytes(buffer);
                result.RngStateSeed = BitConverter.ToInt64(buffer, 0);
                result.RngIncreaseSeed = BitConverter.ToInt64(buffer, 8) | 1;
                yield return result;
            }
        }

        /// <summary>
        ///     Creates a new <see cref="JobCollectionAdapter" /> from the passed <see cref="IModelProject" /> and
        ///     <see cref="KmcJobPackageDescriptionGraph" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="JobPackageDescription"></param>
        /// <returns></returns>
        public static JobCollectionAdapter Create(IModelProject modelProject, KmcJobPackageDescriptionGraph JobPackageDescription)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            if (JobPackageDescription == null) throw new ArgumentNullException(nameof(JobPackageDescription));

            var simulation = modelProject.DataTracker.FindObjectByKey<IKineticSimulation>(JobPackageDescription.SimulationKey)
                             ?? throw new InvalidOperationException("Kinetic simulation key does not exist in the model");
            var baseConfig = (JobPackageDescription.JobBaseDescription ?? new KmcJobDescriptionGraph()).ToInternal(simulation);
            var random = new PcgRandom32(JobPackageDescription.RngSeed ?? simulation.CustomRngSeed);

            var obj = new JobCollectionAdapter
            {
                ModelProject = modelProject,
                Simulation = simulation,
                BaseConfiguration = baseConfig,
                JobPackageDescription = JobPackageDescription,
                Random = random
            };

            return obj;
        }

        /// <summary>
        ///     Creates a new <see cref="JobCollectionAdapter" /> from the passed <see cref="IModelProject" /> and
        ///     <see cref="MmcJobPackageDescriptionGraph" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="jobPackageDescription"></param>
        /// <returns></returns>
        public static JobCollectionAdapter Create(IModelProject modelProject, MmcJobPackageDescriptionGraph jobPackageDescription)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            if (jobPackageDescription == null) throw new ArgumentNullException(nameof(jobPackageDescription));

            var simulation = modelProject.DataTracker.FindObjectByKey<IMetropolisSimulation>(jobPackageDescription.SimulationKey)
                             ?? throw new InvalidOperationException("Metropolis simulation key does not exist in the model");
            var baseConfig = (jobPackageDescription.JobBaseDescription ?? new MmcJobDescriptionGraph()).ToInternal(simulation);
            var random = new PcgRandom32(jobPackageDescription.RngSeed ?? simulation.CustomRngSeed);

            var obj = new JobCollectionAdapter
            {
                ModelProject = modelProject,
                Simulation = simulation,
                BaseConfiguration = baseConfig,
                JobPackageDescription = jobPackageDescription,
                Random = random
            };

            return obj;
        }

        /// <inheritdoc />
        public IEnumerator<JobConfiguration> GetEnumerator()
        {
            return GetJobConfigurations().GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}