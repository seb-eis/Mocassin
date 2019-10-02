using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Tools.UAccess.Readers;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe
{
    /// <summary>
    ///     Provides the context for evaluation of a single MMCFE routine log database
    /// </summary>
    public class MmcfeEvaluationContext : IDisposable
    {
        /// <summary>
        ///     Get the <see cref="ReadOnlyDbContext"/> for the log entry database
        /// </summary>
        public ReadOnlyDbContext DataContext { get; }

        /// <summary>
        ///     Creates a ne <see cref="MmcfeEvaluationContext"/> using the provided <see cref="MmcfeLogDbContext"/>
        /// </summary>
        /// <param name="dataContext"></param>
        public MmcfeEvaluationContext(MmcfeLogDbContext dataContext)
        {
            DataContext = dataContext?.AsReadOnly() ?? throw new ArgumentNullException(nameof(dataContext));
        }

        /// <summary>
        ///     Gets a non-tracking <see cref="IQueryable{T}"/> of the <see cref="MmcfeRoutineLogEntry"/> set
        /// </summary>
        /// <returns></returns>
        public IQueryable<MmcfeRoutineLogEntry> LogSet()
        {
            return DataContext.Set<MmcfeRoutineLogEntry>();
        }

        /// <summary>
        ///     Gets a <see cref="IQueryable{T}"/> of <see cref="MmcfeLogReader"/> for all <see cref="MmcfeRoutineLogEntry"/> entities in the context
        /// </summary>
        /// <returns></returns>
        public IQueryable<MmcfeLogReader> FullReaderSet()
        {
            return CreateReaders(LogSet());
        }

        /// <summary>
        ///     Creates a <see cref="IQueryable{T}"/> of <see cref="MmcfeLogReader"/> from a <see cref="IQueryable{T}"/> of <see cref="MmcfeRoutineLogEntry"/>
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public IQueryable<MmcfeLogReader> CreateReaders(IQueryable<MmcfeRoutineLogEntry> entries)
        {
            return entries.Select(x => MmcfeLogReader.Create(x.StateBytes, x.HistogramBytes, x.ParameterBytes));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DataContext.Dispose();
        }

        /// <summary>
        ///     Opens the provided filepath an <see cref="MmcfeLogDbContext"/> and returns a matching <see cref="MmcfeEvaluationContext"/>
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static MmcfeEvaluationContext OpenFile(string filepath)
        {
            return new MmcfeEvaluationContext(SqLiteContext.OpenDatabase<MmcfeLogDbContext>(filepath));
        }
    }
}