using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.SQLiteCore;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe
{
    /// <summary>
    ///     Represents a <see cref="SqLiteContext{T1}"/> for large MMCFE log collection databases with result and meta data
    /// </summary>
    public class MmcfeLogCollectionDbContext : SqLiteContext<MmcfeLogCollectionDbContext>
    {
        /// <summary>
        ///     Get or set the <see cref="DbSet{TEntity}"/> of <see cref="MmcfeExtendedLogEntry"/> that store the routine results
        /// </summary>
        public DbSet<MmcfeExtendedLogEntry> LogEntries { get; set; }

        /// <summary>
        ///     Get or set the <see cref="DbSet{TEntity}"/> of <see cref="MmcfeLogMetaEntry"/> that store the log meta information
        /// </summary>
        public DbSet<MmcfeLogMetaEntry> MetaEntries { get; set; }

        /// <summary>
        ///     Get or set the <see cref="DbSet{TEntity}"/> of <see cref="MmcfeLogEnergyEntry"/> that store the energetic results
        /// </summary>
        public DbSet<MmcfeLogEnergyEntry> EnergyEntries { get; set; }

        /// <inheritdoc />
        public MmcfeLogCollectionDbContext(string optionsBuilderParameterString)
            : base(optionsBuilderParameterString)
        {
        }

        /// <summary>
        ///     Creates a database copy to a target location and removes the binary raw data entries
        /// </summary>
        /// <param name="filePath"></param>
        public void CopyDatabaseWithoutRawData(string filePath)
        {
            using (var copyContext = OpenDatabase<MmcfeLogCollectionDbContext>(filePath, true))
            {
                copyContext.EnergyEntries.AddRange(EnergyEntries.AsNoTracking());
                copyContext.MetaEntries.AddRange(MetaEntries.AsNoTracking());
                copyContext.LogEntries.AddRange(LogEntries.AsNoTracking().Action(x =>
                { 
                    x.StateBytes = null;
                    x.HistogramBytes = null;
                    x.ParameterBytes = null;
                }));
                copyContext.SaveChanges();
            }
        }
    }
}