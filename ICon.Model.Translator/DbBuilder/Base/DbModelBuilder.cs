using System;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.DbBuilder
{
    /// <summary>
    ///     Abstract base class for implementations of database model builders
    /// </summary>
    public abstract class DbModelBuilder
    {
        /// <summary>
        ///     The project model context that the builder uses
        /// </summary>
        protected IProjectModelContext ModelContext { get; }

        /// <summary>
        ///     Creates new database model builder for the provided model context
        /// </summary>
        /// <param name="modelContext"></param>
        protected DbModelBuilder(IProjectModelContext modelContext)
        {
            ModelContext = modelContext ?? throw new ArgumentNullException(nameof(modelContext));
        }
    }
}