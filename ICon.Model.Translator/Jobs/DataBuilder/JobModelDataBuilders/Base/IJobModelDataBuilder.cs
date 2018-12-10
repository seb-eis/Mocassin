using System.Collections.Generic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     Job model data builder that converts job blueprints into the required simulation database interop objects of the
    ///     job model
    /// </summary>
    public interface IJobModelDataBuilder
    {
        /// <summary>
        ///     Get or set the simulation settings that should be used to check the validity of job properties
        /// </summary>
        MocassinSimulationSettings SimulationSettings { get; set; }

        /// <summary>
        ///     Converts a job package blueprint into a sequence of job model data objects
        /// </summary>
        /// <param name="jobPackageBlueprint"></param>
        /// <returns></returns>
        IEnumerable<JobModelData> Build(IJobPackageBlueprint jobPackageBlueprint);
    }
}