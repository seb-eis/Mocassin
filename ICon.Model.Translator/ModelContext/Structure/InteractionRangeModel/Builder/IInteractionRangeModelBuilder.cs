using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Builder for the interaction range model of the structure model context
    /// </summary>
    public interface IInteractionRangeModelBuilder
    {
        /// <summary>
        ///     Builds the interaction range model for the passed model project
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        IInteractionRangeModel BuildModel(IModelProject modelProject);
    }
}