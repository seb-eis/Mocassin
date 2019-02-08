using System;
using System.Reactive;
using System.Threading.Tasks;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.Model.Translator.Optimization;

namespace Mocassin.Model.Translator.EntityBuilder
{
    /// <summary>
    ///     Database entity builder that converts <see cref="IJobCollection" /> build instructions into
    ///     <see cref="SimulationJobPackageModel" /> database translations
    /// </summary>
    public interface IJobDbEntityBuilder
    {
        /// <summary>
        ///    Get the <see cref="IObservable{T}" /> that informs about every build job
        /// </summary>
        IObservable<int> WhenJobIsBuild { get; }

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

        /// <summary>
        ///     Attaches a simulation data optimizer to the package build process
        /// </summary>
        /// <param name="postBuildOptimizer"></param>
        void AddPostBuildOptimizer(IPostBuildOptimizer postBuildOptimizer);
    }
}