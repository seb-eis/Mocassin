using Mocassin.Model.Translator.Jobs;
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
        ///     Optimizes the completely build passed simulation job package data in the context of the model project and returns
        ///     the set of invalidated <see cref="SimulationExecutionFlags" />
        /// </summary>
        /// <param name="modelContext"></param>
        /// <param name="jobPackage"></param>
        SimulationExecutionFlags Run(IProjectModelContext modelContext, SimulationJobPackageModel jobPackage);
    }
}