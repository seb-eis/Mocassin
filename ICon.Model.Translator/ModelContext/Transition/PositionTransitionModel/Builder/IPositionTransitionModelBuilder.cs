using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Builder for the transition position model that fully describes which transitions are possible on each unit cell position
    /// </summary>
    public interface IPositionTransitionModelBuilder
    {
        /// <summary>
        /// Builds the list of all transition position models that are described within the passed transition model context
        /// and a task that completes when the transition model build is completed
        /// </summary>
        /// <param name="modelContext"></param>
        /// <param name="transitionBuildTask"></param>
        /// <returns></returns>
        IList<IPositionTransitionModel> BuildModels(ITransitionModelContext modelContext, Task transitionBuildTask);
    }
}