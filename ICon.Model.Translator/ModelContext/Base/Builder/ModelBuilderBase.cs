using ICon.Model.ProjectServices;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Abstract base class for model component builders
    /// </summary>
    public abstract class ModelBuilderBase
    {
        /// <summary>
        /// The project instance used for model reference access
        /// </summary>
        protected IProjectServices ProjectServices { get; }

        /// <summary>
        /// Create new model builder base with the provided project access
        /// </summary>
        /// <param name="projectServices"></param>
        protected ModelBuilderBase(IProjectServices projectServices)
        {
            ProjectServices = projectServices;
        }
    }
}