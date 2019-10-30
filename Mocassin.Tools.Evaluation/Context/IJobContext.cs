using Mocassin.Model.Translator;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.Tools.UAccess.Readers;

namespace Mocassin.Tools.Evaluation.Context
{
    /// <summary>
    ///     Provides a common interface for basic job evaluation of the binary simulation state
    /// </summary>
    public interface IJobContext
    {
        /// <summary>
        ///     Get the <see cref="IProjectModelContext" /> of this job
        /// </summary>
        IProjectModelContext ModelContext { get; }

        /// <summary>
        ///     Get the <see cref="ISimulationModel" /> of this job
        /// </summary>
        ISimulationModel SimulationModel { get; }

        /// <summary>
        ///     Get a <see cref="McsContentReader" /> to read the contents of the binary state
        /// </summary>
        McsContentReader McsReader { get; }
    }
}