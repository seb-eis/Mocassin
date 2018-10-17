using System.Collections.Generic;
using Mocassin.Symmetry.Analysis;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    /// Builder for position models that fully described an extended position environment and properties
    /// </summary>
    public interface IPositionModelBuilder
    {
        /// <summary>
        /// Creates all position models for the passed set of environment models
        /// </summary>
        /// <param name="environmentModels"></param>
        /// <returns></returns>
        IList<IPositionModel> BuildModels(IList<IEnvironmentModel> environmentModels);
    }
}