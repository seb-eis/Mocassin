using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    /// Abstract base class for model component builders
    /// </summary>
    public abstract class ModelBuilderBase
    {
        /// <summary>
        /// The project instance used for model reference access
        /// </summary>
        protected IModelProject ModelProject { get; }

        /// <summary>
        /// Create new model builder base with the provided project access
        /// </summary>
        /// <param name="modelProject"></param>
        protected ModelBuilderBase(IModelProject modelProject)
        {
            ModelProject = modelProject;
        }
    }
}