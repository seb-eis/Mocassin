using ICon.Model.ProjectServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Abstract base classs for model context builder implementations that expand the refernce data into the full context
    /// </summary>
    public abstract class ModelContextBuilderBase<TContext> where TContext : class
    {
        /// <summary>
        /// The model context instance that is currently build
        /// </summary>
        protected TContext ModelContext { get; set; }

        /// <summary>
        /// The project service for access to the reference model data
        /// </summary>
        public IProjectServices ProjectServices { get; set; }

        /// <summary>
        /// The project model context builder for access to affiliated model context build processes
        /// </summary>
        public IProjectModelContextBuilder ProjectModelContextBuilder { get; set; }

        /// <summary>
        /// Create new model context builder with the provided project access and default internally defined builders
        /// </summary>
        /// <param name="projectModelContextBuilder"></param>
        protected ModelContextBuilderBase(IProjectModelContextBuilder projectModelContextBuilder)
        {
            ProjectModelContextBuilder = projectModelContextBuilder ?? throw new ArgumentNullException(nameof(projectModelContextBuilder));
            ProjectServices = projectModelContextBuilder.ProjectModelContext.ProjectServices;
        }

        /// <summary>
        /// Builds the context of specified type from the currently linked project reference data
        /// </summary>
        public async Task<TContext> CreateNewContext<T1>() where T1 : TContext, new()
        {
            ModelContext = new T1();
            await Task.Run(PopulateContext);
            return ModelContext;
        }

        /// <summary>
        /// Rebuilds the passed model context instead of creating a new one
        /// </summary>
        /// <param name="context"></param>
        public virtual async Task RebuildContext(TContext context)
        {
            ModelContext = context ?? throw new ArgumentNullException(nameof(context));
            await Task.Run(PopulateContext);
        }

        /// <summary>
        /// Builds the object tree of the currently set energy model context context
        /// </summary>
        protected abstract void PopulateContext();
    }
}
