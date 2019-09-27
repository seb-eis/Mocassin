using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.SQLiteCore;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe
{
    /// <summary>
    ///     The <see cref="SqLiteContext{T1}"/> for MMCFE routine log databases
    /// </summary>
    public class MmcfeLogDbContext : SqLiteContext<MmcfeLogDbContext>
    {
        /// <summary>
        ///     Get or set the <see cref="DbSet{TEntity}"/> of <see cref="MmcfeRoutineLogEntry"/>
        /// </summary>
        public DbSet<MmcfeRoutineLogEntry> RoutineLogEntries { get; set; }

        /// <inheritdoc />
        public MmcfeLogDbContext(string optionsBuilderParameterString)
            : base(optionsBuilderParameterString)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MmcfeRoutineLogEntry>().ToTable("LogEntries");
            base.OnModelCreating(modelBuilder);
        }
    }
}