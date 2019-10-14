using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Mathematics.Comparers;
using Mocassin.Tools.Evaluation.PlotData;
using Mocassin.Tools.Evaluation.Queries.Base;

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
        ///     Get a <see cref="IReadOnlyList{T}" /> of all <see cref="MmcfeEnergyDataPoint" /> entries. Getting this value will
        ///     force load all data points
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
        public IDictionary<string, IDictionary<double, MmcfeEnergyDataPoint>> GroupDataPointsByDopingByTemperature(
            IEqualityComparer<double> temperatureComparer = null)
        {
            temperatureComparer = temperatureComparer ?? NumericComparer.CreateRangedCombined();
            return DataPoints
                .GroupBy(x => x.MetaEntry.DopingInfo)
                .ToDictionary(x => x.Key,
                    y => (IDictionary<double, MmcfeEnergyDataPoint>) y.ToDictionary(arg => arg.EnergyState.Temperature, arg => arg, temperatureComparer));
        }

        /// <summary>
        ///     Create a two level dictionary grouping based on temperature information and then doping value subgroups
        /// </summary>
        /// <returns></returns>
        public IDictionary<double, IDictionary<string, MmcfeEnergyDataPoint>> GroupDataPointsByTemperatureByDoping(
            IEqualityComparer<double> temperatureComparer = null)
        {
            temperatureComparer = temperatureComparer ?? NumericComparer.CreateRangedCombined();
            return DataPoints
                .GroupBy(x => x.EnergyState.Temperature, temperatureComparer)
                .ToDictionary(x => x.Key, y => (IDictionary<string, MmcfeEnergyDataPoint>) y.ToDictionary(arg => arg.MetaEntry.DopingInfo, arg => arg),
                    temperatureComparer);
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
        ///     Gets a per defect <see cref="MmcfeEnergyState" /> plot information where all values are relative to entry with the
        ///     lowest defect particle count
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <param name="fixedDopingValue"></param>
        /// <param name="variableDopingId"></param>
        /// <param name="defectParticleId"></param>
        /// <param name="fixedDopingId"></param>
        /// <returns></returns>
        public PlotData2D<double, MmcfeEnergyState> GetRelativeChangePerDefectPlotData2D(IDictionary<string, MmcfeEnergyDataPoint> dataPoints,
            int fixedDopingId, double fixedDopingValue, int variableDopingId, int defectParticleId)
        {
            var keySelectorFunction = GetDopingSelectorFunction(fixedDopingId, fixedDopingValue);
            var baseData = dataPoints.Where(x => keySelectorFunction(x.Key)).OrderBy(x => x.Value.ParticleCounts[defectParticleId]).ToList();
            var baseState = baseData[0].Value.EnergyState;

            var result = new PlotData2D<double, MmcfeEnergyState>(baseData.Count);
            foreach (var pair in baseData)
            {
                var dopingValue = ParseDopingValue(pair.Value.MetaEntry.DopingInfo, variableDopingId);
                var defectCount = pair.Value.ParticleCounts[defectParticleId];
                var y = pair.Value.EnergyState.AsRelative(baseState).AsPerDefect(defectCount);
                var errorY = pair.Value.EnergyStateError.AsPerDefect(defectCount);
                result.AddPoint(dopingValue, y, 0, errorY);
            }

            return result;
        }


        /// <summary>
        ///     Gets a per unit cell <see cref="MmcfeEnergyState" /> plot information where all values are relative to entry with the
        ///     lowest defect particle count
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <param name="fixedDopingValue"></param>
        /// <param name="variableDopingId"></param>
        /// <param name="defectParticleId"></param>
        /// <param name="fixedDopingId"></param>
        /// <returns></returns>
        public PlotData2D<double, MmcfeEnergyState> GetRelativeChangePerUnitCellPlotData2D(IDictionary<string, MmcfeEnergyDataPoint> dataPoints,
            int fixedDopingId, double fixedDopingValue, int variableDopingId, int defectParticleId)
        {
            var keySelectorFunction = GetDopingSelectorFunction(fixedDopingId, fixedDopingValue);
            var baseData = dataPoints.Where(x => keySelectorFunction(x.Key)).OrderBy(x => x.Value.ParticleCounts[defectParticleId]).ToList();
            var baseState = baseData[0].Value.EnergyState;

            var result = new PlotData2D<double, MmcfeEnergyState>(baseData.Count);
            foreach (var pair in baseData)
            {
                var cellCount = ParseUnitCellCount(pair.Value.MetaEntry.LatticeInfo);
                var dopingValue = ParseDopingValue(pair.Value.MetaEntry.DopingInfo, variableDopingId);
                var y = pair.Value.EnergyState.AsRelative(baseState).AsPerDefect(cellCount);
                var errorY = pair.Value.EnergyStateError.AsPerDefect(cellCount);
                result.AddPoint(dopingValue, y, 0, errorY);
            }

            return result;
        }

        /// <summary>
        ///     Writes the <see cref="PlotData2D{TX,TY}" /> for a <see cref="MmcfeEnergyState" /> over concentration curve to the
        ///     provided file location. By default the first line is skipped
        /// </summary>
        /// <param name="plotData"></param>
        /// <param name="filePath"></param>
        /// <param name="skipLineCount"></param>
        /// <param name="doubleFormat"></param>
        public void WriteEnergyStateOverConcentrationPlotData2DToFile(PlotData2D<double, MmcfeEnergyState> plotData, string filePath, bool entropyInKb = true,
            int skipLineCount = 1, string doubleFormat = "e13")
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            using (var writer = File.AppendText(filePath))
            {
                var entropyFactor = entropyInKb ? 1.0 / Equations.Constants.BlotzmannEv : 1.0;
                writer.WriteLine("c u error-u f error-f s error-s");
                foreach (var (x, errorX, y, errorY) in plotData.Skip(skipLineCount))
                {
                    writer.Write(x.ToString(doubleFormat));
                    writer.Write(" ");
                    WriteValueWithError(writer, y.InnerEnergy, errorY.InnerEnergy, false, doubleFormat);
                    WriteValueWithError(writer, y.FreeEnergy, errorY.FreeEnergy, false, doubleFormat);
                    WriteValueWithError(writer, y.Entropy * entropyFactor, errorY.Entropy * entropyFactor, true, doubleFormat);
                }
            }
        }

        /// <summary>
        ///     Writes a numeric value with error to a <see cref="StreamWriter" /> using the provided format information
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="error"></param>
        /// <param name="lastColumn"></param>
        /// <param name="valueFormat"></param>
        /// <param name="separator"></param>
        private void WriteValueWithError(StreamWriter writer, double value, double error, bool lastColumn = false, string valueFormat = "e13",
            string separator = " ")
        {
            writer.Write(value.ToString(valueFormat));
            writer.Write(separator);
            writer.Write(error.ToString(valueFormat));
            writer.Write(lastColumn ? Environment.NewLine : separator);
        }

        /// <summary>
        ///     Parses a doping value for the specified id from the doping info <see cref="string" />
        /// </summary>
        /// <param name="dopingString"></param>
        /// <param name="dopingId"></param>
        /// <returns></returns>
        public double ParseDopingValue(string dopingString, int dopingId)
        {
            var str = Regex.Match(dopingString, $"{dopingId}@([^]]+)").Groups[1].Value;
            return double.Parse(str);
        }

        /// <summary>
        ///     Parses the total number of unit cells from the specified cell info string <see cref="string" />
        /// </summary>
        /// <param name="cellString"></param>
        public int ParseUnitCellCount(string cellString)
        {
            var values = cellString.Split(',');
            return int.Parse(values[0]) * int.Parse(values[1]) * int.Parse(values[2]);
        }

        /// <summary>
        ///     Get a <see cref="Func{T1, TResult}" /> selector function that returns true if the provided oping id is at the
        ///     specified value
        /// </summary>
        /// <param name="dopingId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Func<string, bool> GetDopingSelectorFunction(int dopingId, double value)
        {
            var str = $"[{dopingId}@{value}]";
            return x => x.Contains(str);
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
            var (energyState, energyStateError) = EnergyEvaluator.AverageWithSem(data.Select(x => sel2(x).AsStruct()));
            return new MmcfeEnergyDataPoint(data.Count, energyState, energyStateError, sel1(data[0]));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (IsDataSourceDisposeWithObject) (DataSource as IDisposable)?.Dispose();
        }
    }
}