using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.SQLiteCore;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     The <see cref="SqLiteContext{T1}"/> for a <see cref="SpaceGroupEntity"/> database
    /// </summary>
    public sealed class SpaceGroupContext : SqLiteContext<SpaceGroupContext>
    {
        /// <inheritdoc />
        public SpaceGroupContext(string optionsBuilderParameterString)
            : base(optionsBuilderParameterString)
        {
            Database.EnsureCreated();
        }

        /// <summary>
        ///     The <see cref="DbSet{TEntity}"/> of <see cref="SpaceGroupEntity"/> instances
        /// </summary>
        public DbSet<SpaceGroupEntity> SpaceGroups { get; set; }

        /// <summary>
        ///     The <see cref="DbSet{TEntity}"/> of <see cref="SymmetryOperationEntity"/> instances
        /// </summary>
        public DbSet<SymmetryOperationEntity> SymmetryOperations { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SpaceGroupEntity>().Property(e => e.CrystalType).HasConversion<int>();
            modelBuilder.Entity<SpaceGroupEntity>().Property(e => e.CrystalVariation).HasConversion<int>();
            base.OnModelCreating(modelBuilder);
        }
    }
}