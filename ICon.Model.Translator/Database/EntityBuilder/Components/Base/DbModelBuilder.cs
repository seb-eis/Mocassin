using System;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.EntityBuilder
{
    /// <summary>
    ///     Abstract base class for implementations of simulation db entity builders
    /// </summary>
    public abstract class DbEntityBuilder
    {
        /// <summary>
        ///     The project model context that the builder uses
        /// </summary>
        protected IProjectModelContext ModelContext { get; }

        /// <summary>
        ///     Creates new database entity builder for the provided model context
        /// </summary>
        /// <param name="modelContext"></param>
        protected DbEntityBuilder(IProjectModelContext modelContext)
        {
            ModelContext = modelContext ?? throw new ArgumentNullException(nameof(modelContext));
        }
    }
}