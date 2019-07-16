﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Random;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.Optimization;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Adapter class to provide <see cref="JobPackageDescriptionGraph" /> data as a <see cref="IJobCollection" /> object
    ///     for the
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
        ///     Get the <see cref="JobPackageDescriptionGraph" /> that is used as a <see cref="JobPackageDescriptionGraph" />
        ///     source
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
        public int CollectionId { get; set; }

        /// <inheritdoc />
        public ISimulation GetSimulation()
        {
            return Simulation;
        }

        /// <inheritdoc />
        public IEnumerable<IPostBuildOptimizer> GetPostBuildOptimizers()
        {
            return JobPackageDescription.GetManualOptimizers().Select(x => x.ToInternal(ModelProject));
        }

        /// <summary>
        ///     Get the sequence of <see cref="JobConfiguration"/> instances of the <see cref="IJobCollection"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JobConfiguration> GetJobConfigurations()
        {
            var jobCount = JobPackageDescription.JobCountPerConfig is null
                ? Simulation.JobCount
                : int.Parse(JobPackageDescription.JobCountPerConfig);

            var configIndex = 0;
            return JobPackageDescription.GetConfigurations()
                .SelectMany(x => ExpandToJobCount(x.ToInternal(BaseConfiguration, ModelProject, configIndex++), jobCount))
                .Action(x => x.CollectionName = JobPackageDescription.Name);
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
                result.JobIndex = i;
                yield return result;
            }
        }

        /// <summary>
        ///     Creates a new <see cref="JobCollectionAdapter" /> from the passed <see cref="IModelProject" /> and
        ///     <see cref="KmcJobPackageDescriptionGraph" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="jobPackageDescription"></param>
        /// <returns></returns>
        public static JobCollectionAdapter Create(IModelProject modelProject, KmcJobPackageDescriptionGraph jobPackageDescription)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));
            if (jobPackageDescription == null) throw new ArgumentNullException(nameof(jobPackageDescription));

            var simulation = modelProject.DataTracker.FindObjectByKey<IKineticSimulation>(jobPackageDescription.Simulation.Key)
                             ?? throw new InvalidOperationException("Kinetic simulation key does not exist in the model");

            var baseConfig = (jobPackageDescription.JobBaseDescription ?? new KmcJobDescriptionGraph()).ToInternal(simulation);
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

            var simulation = modelProject.DataTracker.FindObjectByKey<IMetropolisSimulation>(jobPackageDescription.Simulation.Key)
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