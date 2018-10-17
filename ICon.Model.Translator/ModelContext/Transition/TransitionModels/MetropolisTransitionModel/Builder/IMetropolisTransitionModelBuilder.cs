using System.Collections.Generic;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a builder for metropolis transition models and affiliated data types
    /// </summary>
    public interface IMetropolisTransitionModelBuilder
    {
        /// <summary>
        /// Builds and links the complete metropolis transition model collection from the passed set of transitions
        /// </summary>
        /// <param name="transitions"></param>
        /// <returns></returns>
        IList<IMetropolisTransitionModel> BuildModels(IEnumerable<IMetropolisTransition> transitions);
    }
}