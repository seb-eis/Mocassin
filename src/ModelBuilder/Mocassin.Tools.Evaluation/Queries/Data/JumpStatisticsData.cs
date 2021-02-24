using System;
using System.Collections.Generic;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.Tools.Evaluation.Helper;
using Mocassin.Tools.UAccess.Readers;
using Mocassin.Tools.UAccess.Readers.Data;

namespace Mocassin.Tools.Evaluation.Queries.Data
{
    /// <summary>
    ///     A data class for storing jump statistic results for <see cref="IGlobalTrackerModel" /> instances of a simulation
    /// </summary>
    public class JumpStatisticsData
    {
        /// <summary>
        ///     Get the <see cref="McsContentReader" /> that belongs to the simulation
        /// </summary>
        private McsContentReader McsReader { get; }

        /// <summary>
        ///     Get the <see cref="IGlobalTrackerModel" /> for which the statistics data is valid
        /// </summary>
        public IGlobalTrackerModel GlobalTrackerModel { get; }

        /// <summary>
        ///     Get the simulation temperature in [K]
        /// </summary>
        public double Temperature { get; }

        /// <summary>
        ///     Get the simulated timespan in [s]
        /// </summary>
        public double SimulatedTime { get; }

        /// <summary>
        ///     Get the total number of jump attempts
        /// </summary>
        public long AttemptCount { get; }

        /// <summary>
        ///     Get the number of particles in the affiliated ensemble
        /// </summary>
        public int EnsembleSize { get; }

        /// <summary>
        ///     Creates a new <see cref="JumpStatisticsData" /> for a <see cref="IGlobalTrackerModel" />
        /// </summary>
        /// <param name="mcsReader"></param>
        /// <param name="globalTrackerModel"></param>
        /// <param name="temperature"></param>
        /// <param name="ensembleSize"></param>
        public JumpStatisticsData(McsContentReader mcsReader, IGlobalTrackerModel globalTrackerModel, double temperature, int ensembleSize)
        {
            McsReader = mcsReader;
            GlobalTrackerModel = globalTrackerModel;
            Temperature = temperature;
            EnsembleSize = ensembleSize;
            SimulatedTime = mcsReader.ReadMetaData().SimulatedTime;
            AttemptCount = CountAttempts();
        }

        /// <summary>
        ///     Reads the raw <see cref="McsJumpStatistic" /> struct from the simulation state
        /// </summary>
        /// <returns></returns>
        public McsJumpStatistic GetMcsJumpStatistic() => McsReader.ReadJumpStatistics()[GlobalTrackerModel.ModelId];

        /// <summary>
        ///     Counts the number of jump attempts
        /// </summary>
        /// <returns></returns>
        public long CountAttempts()
        {
            var histogram = GetMcsJumpStatistic().JumpEnergyHistogram;
            return histogram.AboveMaxCount + histogram.BelowMinCount + CountNonOverflowAttempts();
        }

        /// <summary>
        ///     Count the number of jump attempts that are not above or below the histogram range
        /// </summary>
        /// <returns></returns>
        public long CountNonOverflowAttempts()
        {
            var histogram = GetMcsJumpStatistic().JumpEnergyHistogram;
            var count = 0L;
            foreach (var counter in histogram.Counters) count += counter;

            return count;
        }

        /// <summary>
        ///     Estimates the actual jump count by applying Boltzmann statistics to the attempt histogram
        /// </summary>
        /// <returns></returns>
        public double EstimateJumpCount()
        {
            var histogram = GetMcsJumpStatistic().JumpEnergyHistogram;
            var energy = histogram.MinEnergyValue;
            var energyStep = 1.0 / histogram.InverseStepping;
            var normFactor = McsReader.ReadMetaData().JumpNormalization;
            var count = (double) histogram.BelowMinCount;
            
            foreach (var counter in histogram.Counters)
            {
                var corrected = normFactor * counter * Math.Exp(-energy / (Temperature * Equations.Constants.BoltzmannEv));
                energy += energyStep;
                count += corrected;
            }

            return count;
        }

        /// <summary>
        ///     Estimates the actual migration rate using the jump count estimation
        /// </summary>
        /// <returns></returns>
        public double EstimateMigrationRate() => EstimateJumpCount() / (SimulatedTime * EnsembleSize);

        /// <summary>
        ///     Enumerates all entries from the edge energy histogram excluding the overflow counters
        /// </summary>
        /// <returns></returns>
        public (double Energy, long Count)[] GetEdgeEntries() => GetHistogramValues(GetMcsJumpStatistic().EdgeEnergyHistogram);

        /// <summary>
        ///     Enumerates all entries from the positive configuration contribution histogram excluding the overflow counters
        /// </summary>
        /// <returns></returns>
        public (double Energy, long Count)[] GetPositiveConfigurationsEntries() => GetHistogramValues(GetMcsJumpStatistic().PositiveConformationEnergyHistogram);

        /// <summary>
        ///     Enumerates all entries from the negative configuration contribution histogram excluding the overflow counters
        /// </summary>
        /// <returns></returns>
        public (double Energy, long Count)[] GetNegativeConfigurationEntries() => GetHistogramValues(GetMcsJumpStatistic().NegativeConformationEnergyHistogram, true);

        /// <summary>
        ///     Enumerates all entries from the total migration barrier histogram excluding the overflow counters
        /// </summary>
        /// <returns></returns>
        public (double Energy, long Count)[] GetMigrationBarrierEntries() => GetHistogramValues(GetMcsJumpStatistic().JumpEnergyHistogram);

        /// <summary>
        ///     Enumerates the energy and counter pairs of a <see cref="McsJumpHistogram" />
        /// </summary>
        /// <param name="histogram"></param>
        /// <param name="isNegative"></param>
        /// <returns></returns>
        private (double Energy, long Count)[] GetHistogramValues(McsJumpHistogram histogram, bool isNegative = false)
        {
            var energyStep = 1.0 / histogram.InverseStepping;
            var factor = (isNegative) ? -1.0 : 1.0;

            var result = new (double, long)[histogram.Counters.Length];
            for (var i = 0; i < histogram.Counters.Length; i++)
            {
                var count = histogram[i];
                var energy = histogram.MinEnergyValue + i * energyStep;
                result[i] = (energy * factor, count);
            }

            return result;
        }
    }
}