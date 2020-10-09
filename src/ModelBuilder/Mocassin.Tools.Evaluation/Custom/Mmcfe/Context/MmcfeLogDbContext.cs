using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.SQLiteCore;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe
{
    /// <summary>
    ///     The <see cref="SqLiteContext" /> for MMCFE routine log databases
    /// </summary>
    public class MmcfeLogDbContext : SqLiteContext
    {
        /// <summary>
        ///     Get or set the <see cref="DbSet{TEntity}" /> of <see cref="MmcfeLogEntry" />
        /// </summary>
        public DbSet<MmcfeLogEntry> LogEntries { get; set; }

        /// <inheritdoc />
        public MmcfeLogDbContext(string optionsBuilderParameterString)
            : base(optionsBuilderParameterString)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MmcfeLogEntry>().ToTable("LogEntries");
            base.OnModelCreating(modelBuilder);
        }
    }
}