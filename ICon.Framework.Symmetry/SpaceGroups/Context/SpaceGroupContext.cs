using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.SQLiteCore;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     The space group SQLite EFCore database context
    /// </summary>
    public sealed class SpaceGroupContext : SqLiteContext<SpaceGroupContext>
    {
        /// <inheritdoc />
        public SpaceGroupContext()
        {
        }

        /// <inheritdoc />
        public SpaceGroupContext(string optionsBuilderParameterString)
            : base(optionsBuilderParameterString)
        {
            Database.EnsureCreated();
        }

        /// <summary>
        ///     Space group database sets
        /// </summary>
        public DbSet<SpaceGroupEntity> SpaceGroups { get; set; }

        /// <summary>
        ///     Symmetry operation database sets
        /// </summary>
        public DbSet<SymmetryOperationEntity> SymmetryOperations { get; set; }

        /// <inheritdoc />
        public override SpaceGroupContext CreateNewContext(string optionsBuilderParameterString)
        {
            return new SpaceGroupContext(optionsBuilderParameterString);
        }
    }
}