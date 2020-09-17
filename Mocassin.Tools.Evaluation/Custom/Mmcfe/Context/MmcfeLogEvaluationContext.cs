using System;
using System.Linq;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Tools.UAccess.Readers;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe
{
    /// <summary>
    ///     Provides the context for evaluation of a single MMCFE routine log database
    /// </summary>
    public class MmcfeLogEvaluationContext : IDisposable
    {
        /// <summary>
        ///     Get the <see cref="ReadOnlyDbContext" /> for the log entry database
        /// </summary>
        public ReadOnlyDbContext DataContext { get; }

        /// <summary>
        ///     Creates a ne <see cref="MmcfeLogEvaluationContext" /> using the provided <see cref="MmcfeLogDbContext" />
        /// </summary>
        /// <param name="dataContext"></param>
        public MmcfeLogEvaluationContext(MmcfeLogDbContext dataContext)
        {
            DataContext = dataContext?.AsReadOnly() ?? throw new ArgumentNullException(nameof(dataContext));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DataContext.Dispose();
        }

        /// <summary>
        ///     Gets a non-tracking <see cref="IQueryable{T}" /> of the <see cref="MmcfeLogEntry" /> set
        /// </summary>
        /// <returns></returns>
        public IQueryable<MmcfeLogEntry> LogSet() => DataContext.Set<MmcfeLogEntry>();

        /// <summary>
        ///     Gets a <see cref="IQueryable{T}" /> of <see cref="MmcfeLogReader" /> for all <see cref="MmcfeLogEntry" />
        ///     entities in the context
        /// </summary>
        /// <returns></returns>
        public IQueryable<MmcfeLogReader> FullReaderSet() => CreateReaders(LogSet());

        /// <summary>
        ///     Creates a <see cref="IQueryable{T}" /> of <see cref="MmcfeLogReader" /> from a <see cref="IQueryable{T}" /> of
        ///     <see cref="MmcfeLogEntry" />
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public IQueryable<MmcfeLogReader> CreateReaders(IQueryable<MmcfeLogEntry> entries)
        {
            return entries.Select(x => MmcfeLogReader.Create(x.StateBytes, x.HistogramBytes, x.ParameterBytes));
        }

        /// <summary>
        ///     Opens the provided filepath an <see cref="MmcfeLogDbContext" /> and returns a matching
        ///     <see cref="MmcfeLogEvaluationContext" />
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static MmcfeLogEvaluationContext OpenFile(string filepath) =>
            new MmcfeLogEvaluationContext(SqLiteContext.OpenDatabase<MmcfeLogDbContext>(filepath));
    }
}