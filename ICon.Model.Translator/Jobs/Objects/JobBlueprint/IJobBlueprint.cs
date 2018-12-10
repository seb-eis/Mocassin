using System.Collections.Generic;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     A single job building instruction that carries simulation build instructions for a single simulation job
    /// </summary>
    public interface IJobBlueprint
    {
        /// <summary>
        ///     Get or set the package blueprint that defines the parent properties of the simulation
        /// </summary>
        IJobPackageBlueprint PackageBlueprint { get; set; }

        /// <summary>
        ///     Get or set the affiliated list of job properties
        /// </summary>
        IList<IJobProperty> JobProperties { get; set; }
    }
}