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
    public abstract class ModelContextBuilder<TContext> where TContext : class
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
        /// Create new model context builder that uses the provided project access
        /// </summary>
        /// <param name="projectServices"></param>
        protected ModelContextBuilder(IProjectServices projectServices)
        {
            ProjectServices = projectServices ?? throw new ArgumentNullException(nameof(projectServices));
        }

        /// <summary>
        /// Builds the context of specififed type from the currently linked project reference data
        /// </summary>
        public async Task<TContext> BuildNewContext<T1>() where T1 : TContext, new()
        {
            ModelContext = new T1();
            await Task.Run(() => PopulateContext());
            return ModelContext;
        }

        /// <summary>
        /// Thebuilds the passed model context instead of creating a new one
        /// </summary>
        /// <param name="context"></param>
        public virtual async Task RebuildContext(TContext context)
        {
            ModelContext = context ?? throw new ArgumentNullException(nameof(context));
            await Task.Run(() => PopulateContext());
        }

        /// <summary>
        /// Builds the object tree of the currently set energy model context context
        /// </summary>
        protected abstract void PopulateContext();
    }
}
