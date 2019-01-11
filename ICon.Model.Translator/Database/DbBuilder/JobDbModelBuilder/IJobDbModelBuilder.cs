using System.Threading.Tasks;
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
        ///     Get or seth the used project model context for translation
        /// </summary>
        IProjectModelContext ProjectModelContext { get; set; }

        /// <summary>
        ///     Builds the simulation package model for the passed simulation job collection
        /// </summary>
        /// <param name="jobCollection"></param>
        /// <returns></returns>
        SimulationJobPackageModel BuildJobPackageModel(IJobCollection jobCollection);

        /// <summary>
        ///     Builds the simulation package model for the passed simulation job collection asynchronously
        /// </summary>
        /// <param name="jobCollection"></param>
        /// <returns></returns>
        Task<SimulationJobPackageModel> BuildJobPackageModelAsync(IJobCollection jobCollection);
    }
}