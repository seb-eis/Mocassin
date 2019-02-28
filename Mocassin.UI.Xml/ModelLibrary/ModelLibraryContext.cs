using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.SQLiteCore;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.ParticleModel;

namespace Mocassin.UI.Xml.ModelLibrary
{
    /// <summary>
    ///     The <see cref="DbContext" /> for mocassin model libraries that stored user projects
    /// </summary>
    public sealed class ModelLibraryContext : SqLiteContext<ModelLibraryContext>
    {
        /// <inheritdoc />
        private ModelLibraryContext(string optionsBuilderParameterString)
            : base(optionsBuilderParameterString)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        /// <inheritdoc />
        public ModelLibraryContext()
        {
        }

        /// <inheritdoc />
        public override ModelLibraryContext CreateNewContext(string optionsBuilderParameterString)
        {
            return new ModelLibraryContext(optionsBuilderParameterString);
        }
    }
}