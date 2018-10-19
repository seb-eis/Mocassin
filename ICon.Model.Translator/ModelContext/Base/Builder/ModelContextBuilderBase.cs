using System;
using System.Threading.Tasks;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public abstract class ModelContextBuilderBase<TContext> : IModelContextBuilder<TContext>
        where TContext : class
    {
        /// <inheritdoc />
        public Task<TContext> BuildTask { get; protected set; }

        /// <summary>
        ///     The project service for access to the reference model data
        /// </summary>
        public IModelProject ModelProject { get; set; }

        /// <summary>
        ///     The project model context builder for access to affiliated model context build processes
        /// </summary>
        public IProjectModelContextBuilder ProjectModelContextBuilder { get; set; }

        /// <summary>
        ///     Create new model context builder with the provided project access and default internally defined builders
        /// </summary>
        /// <param name="projectModelContextBuilder"></param>
        protected ModelContextBuilderBase(IProjectModelContextBuilder projectModelContextBuilder)
        {
            ProjectModelContextBuilder = projectModelContextBuilder ?? throw new ArgumentNullException(nameof(projectModelContextBuilder));
            ModelProject = projectModelContextBuilder.ModelProject;
        }

        /// <inheritdoc />
        public virtual Task<TContext> BuildContext()
        {
            return RebuildContext(GetEmptyDefaultContext());
        }

        /// <inheritdoc />
        public virtual Task BuildLinkDependentComponents()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task<TContext> RebuildContext(TContext modelContext)
        {
            SetNullBuildersToDefault();
            BuildTask = Task.Run(() => PopulateContext(modelContext));
            return BuildTask;
        }

        /// <summary>
        ///     Populates the passed context and returns the object on completion
        /// </summary>
        protected abstract TContext PopulateContext(TContext modelContext);

        /// <summary>
        ///     Creates a new empty context as defined in the implementing builder
        /// </summary>
        /// <returns></returns>
        protected abstract TContext GetEmptyDefaultContext();

        /// <summary>
        ///     Sets all unset builder instances to the internally defined default builder system
        /// </summary>
        protected abstract void SetNullBuildersToDefault();
    }
}