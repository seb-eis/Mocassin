using System.Collections.Generic;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a model builder for kinetic transition models and affiliated data types
    /// </summary>
    public interface IKineticTransitionModelBuilder
    {
        /// <summary>
        /// Builds and links the complete kinetic transition model collection from the passed set of transitions
        /// </summary>
        /// <param name="transitions"></param>
        /// <returns></returns>
        IList<IKineticTransitionModel> BuildModels(IList<IKineticTransition> transitions);
    }
}