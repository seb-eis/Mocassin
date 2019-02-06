using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.Optimization
{
    /// <summary>
    ///     Common interface for simulation parametrization data optimizers that optimize the contents of a simulation package
    ///     for runtime after the build process
    /// </summary>
    public interface IPostBuildOptimizer
    {
        /// <summary>
        ///     Optimizes the completely build passed simulation job package data in the context of the model project
        /// </summary>
        /// <param name="modelContext"></param>
        /// <param name="jobPackage"></param>
        void Run(IProjectModelContext modelContext, SimulationJobPackageModel jobPackage);
    }
}