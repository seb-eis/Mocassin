using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Mathematics.Comparers;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe
{
    /// <summary>
    ///     An energy hyper surface evaluator that provides evaluation access to multi-doping multi-temperature MMCFE result
    ///     databases
    /// </summary>
    public class MmcfeEnergyDataPointEvaluator : IDisposable
    {
        private IReadOnlyList<MmcfeEnergyDataPoint> dataPoints;

        /// <summary>
        ///     Get the <see cref="MmcfeEnergyEvaluator" /> used by the system
        /// </summary>
        private MmcfeEnergyEvaluator EnergyEvaluator { get; }

        /// <summary>
        ///     Get a boolean flag if the <see cref="DataSource" /> should be disposed if the object is disposed
        /// </summary>
        private bool IsDataSourceDisposeWithObject { get; }

        /// <summary>
        ///     Get the <see cref="IQueryableDataSource" /> that provides the evaluation data
        /// </summary>
        private IQueryableDataSource DataSource { get; }

        /// <summary>
        ///     Get a <see cref="IReadOnlyList{T}"/> of all <see cref="MmcfeEnergyDataPoint"/> entries. Getting this value will force load all data points
        /// </summary>
        public IReadOnlyList<MmcfeEnergyDataPoint> DataPoints => dataPoints ?? (dataPoints = SelectEnergyDataPoints().ToList());

        /// <summary>
        ///     Creates a new <see cref="MmcfeEnergyDataPointEvaluator" /> using the provided
        ///     <see cref="IQueryableDataSource" /> with an optional flag
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="isDataSourceDisposeWithObject"></param>
        public MmcfeEnergyDataPointEvaluator(IQueryableDataSource dataSource, bool isDataSourceDisposeWithObject = true)
        {
            DataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
            IsDataSourceDisposeWithObject = isDataSourceDisposeWithObject;
            EnergyEvaluator = new MmcfeEnergyEvaluator();
        }

        /// <summary>
        ///     Causes the evaluator to load all data points into memory
        /// </summary>
        public void LoadDataPoints()
        {
            dataPoints = SelectEnergyDataPoints().ToList();
        }

        /// <summary>
        ///     Selects a set of <see cref="MmcfeEnergyState" /> entries from the data source based on a predicate expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<MmcfeLogEnergyEntry> SelectEnergyEntries(Expression<Func<MmcfeLogEnergyEntry, bool>> predicate)
        {
            return DataSource.Set<MmcfeLogEnergyEntry>().Where(predicate);
        }

        /// <summary>
        ///     Selects the full set of <see cref="MmcfeEnergyState" /> entries from the data source
        /// </summary>
        /// <returns></returns>
        public IQueryable<MmcfeLogEnergyEntry> SelectEnergyEntries()
        {
            return SelectEnergyEntries(x => x.LogEntryId >= 1);
        }

        /// <summary>
        ///     Selects a set of <see cref="MmcfeEnergyDataPoint" /> entries from the data source based on a predicate expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<MmcfeEnergyDataPoint> SelectEnergyDataPoints(Expression<Func<MmcfeLogEnergyEntry, bool>> predicate)
        {
            return SelectEnergyEntries(predicate)
                .GroupBy(x => new {x.LogEntry.MetaEntry.CollectionIndex, x.LogEntry.MetaEntry.ConfigIndex, x.Alpha}, y => new {entry = y, y.LogEntry.MetaEntry})
                .Select(x => CreateDataPoint(x, y => y.MetaEntry, y => y.entry));
        }

        /// <summary>
        ///     Create a a two level dictionary grouping based on doping information and then temperature value subgroups
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, IDictionary<double, MmcfeEnergyDataPoint>> GroupDataPointsByDopingByTemperature(IEqualityComparer<double> temperatureComparer = null)
        {
            temperatureComparer = temperatureComparer ?? NumericComparer.CreateRangedCombined();
            return DataPoints
                .GroupBy(x => x.MetaEntry.DopingInfo)
                .ToDictionary(x => x.Key, y => (IDictionary<double, MmcfeEnergyDataPoint>) y.ToDictionary(arg => arg.EnergyState.Temperature, arg => arg, temperatureComparer));
        }

        /// <summary>
        ///     Create a two level dictionary grouping based on temperature information and then doping value subgroups
        /// </summary>
        /// <returns></returns>
        public IDictionary<double, IDictionary<string, MmcfeEnergyDataPoint>> GroupDataPointsByTemperatureByDoping(IEqualityComparer<double> temperatureComparer = null)
        {
            temperatureComparer = temperatureComparer ?? NumericComparer.CreateRangedCombined();
            return DataPoints
                .GroupBy(x => x.EnergyState.Temperature, temperatureComparer)
                .ToDictionary(x => x.Key, y => (IDictionary<string, MmcfeEnergyDataPoint>) y.ToDictionary(arg => arg.MetaEntry.DopingInfo, arg => arg), temperatureComparer);
        }

        /// <summary>
        ///     Selects the full set of <see cref="MmcfeEnergyDataPoint" /> entries from the data source
        /// </summary>
        /// <returns></returns>
        public IQueryable<MmcfeEnergyDataPoint> SelectEnergyDataPoints()
        {
            return SelectEnergyDataPoints(x => x.LogEntry.MetaEntryId >= 1);
        }

        /// <summary>
        ///     Converts a <see cref="IGrouping{TKey,TElement}" /> of <see cref="MmcfeLogEnergyEntry" /> belonging to the same
        ///     <see cref="MmcfeLogMetaEntry" /> into a <see cref="MmcfeEnergyDataPoint" />
        /// </summary>
        /// <param name="grouping"></param>
        /// <returns></returns>
        protected MmcfeEnergyDataPoint CreateDataPoint<T1, T2>(IGrouping<T1, T2> grouping, Func<T2, MmcfeLogMetaEntry> sel1, Func<T2, MmcfeLogEnergyEntry> sel2)
        {
            var data = grouping.ToList();
            var (energyState, energyStateError) = EnergyEvaluator.CalculateAverage(data.Select(x => sel2(x).AsStruct()));
            return new MmcfeEnergyDataPoint(data.Count, energyState, energyStateError, sel1(data[0]));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (IsDataSourceDisposeWithObject) (DataSource as IDisposable)?.Dispose();
        }
    }
}