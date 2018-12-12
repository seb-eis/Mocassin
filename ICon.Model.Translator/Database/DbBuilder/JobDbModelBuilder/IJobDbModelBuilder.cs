using Mocassin.Framework.Operations;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.DbBuilder
{
    /// <summary>
    ///     Database model builder that converts job model definitions into the required job database model objects
    /// </summary>
    public interface IJobDbModelBuilder
    {
        /// <summary>
        ///     Get or seth the used project model context builder
        /// </summary>
        IProjectModelContextBuilder ProjectModelContextBuilder { get; set; }

        /// <summary>
        ///     Builds the simulation package model for the passed simulation job collection
        /// </summary>
        /// <param name="jobCollection"></param>
        /// <returns></returns>
        SimulationJobPackageModel BuildJobPackageModel(IJobCollection jobCollection);
    }
}