using System;
using System.Collections.Generic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Translator.Jobs
{
    /// <inheritdoc />
    public abstract class JobModelDataBuilder : IJobModelDataBuilder
    {
        /// <summary>
        ///     The name mapping dictionary that assign each property name a value setter
        /// </summary>
        private static IDictionary<string, Action<IJobProperty>> JobPropertySetterMapping { get; }

        /// <summary>
        ///     The base job info struct that is used for each build operation
        /// </summary>
        protected CJobInfo BaseJobInfo { get; set; }

        /// <inheritdoc />
        public MocassinSimulationSettings SimulationSettings { get; set; }

        /// <summary>
        ///     Static constructor for the job model data builder
        /// </summary>
        static JobModelDataBuilder()
        {
            JobPropertySetterMapping = GetJobPropertySetterMapping();
        }


        /// <inheritdoc />
        public IEnumerable<JobModelData> Build(IJobPackageBlueprint jobPackageBlueprint)
        {
            SetBaseJobHeader(jobPackageBlueprint);
            SetBaseJobInfo(jobPackageBlueprint);

            foreach (var jobBlueprint in jobPackageBlueprint) yield return Build(jobBlueprint);
        }

        /// <summary>
        ///     Build a single job model data for the passed job blueprint object
        /// </summary>
        /// <param name="jobBlueprint"></param>
        /// <returns></returns>
        protected JobModelData Build(IJobBlueprint jobBlueprint)
        {
            var buildBase = GetNewBuildBase();

            return buildBase;
        }

        /// <summary>
        ///     Get a new build base that is set to the default values of the currently translated job package blueprint
        /// </summary>
        /// <returns></returns>
        protected abstract JobModelData GetNewBuildBase();

        /// <summary>
        ///     Set the base job header so that it matches the properties of the provided package blueprint
        /// </summary>
        /// <param name="jobPackageBlueprint"></param>
        protected abstract void SetBaseJobHeader(IJobPackageBlueprint jobPackageBlueprint);

        /// <summary>
        ///     Set the base job info so that it matches the properties of the provided package blueprint
        /// </summary>
        /// <param name="jobPackageBlueprint"></param>
        protected void SetBaseJobInfo(IJobPackageBlueprint jobPackageBlueprint)
        {
            BaseJobInfo = new CJobInfo();
        }

        /// <summary>
        ///     Creates the job property setter mapping dictionary
        /// </summary>
        /// <returns></returns>
        private static IDictionary<string, Action<IJobProperty>> GetJobPropertySetterMapping()
        {
            return null;
        }
    }
}