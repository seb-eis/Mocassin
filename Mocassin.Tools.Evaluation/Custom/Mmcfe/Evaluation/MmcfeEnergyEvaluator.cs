using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Tools.Evaluation.Queries.Base;
using Mocassin.Tools.UAccess.Readers;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe
{
    /// <summary>
    ///     Evaluator for the energy states of an MMCFE result sequence
    /// </summary>
    public class MmcfeEnergyEvaluator
    {
        /// <summary>
        ///     Calculates the complete set of <see cref="MmcfeEnergyState" /> objects resulting from a
        ///     <see cref="IReadOnlyList{T}" /> of <see cref="MmcfeLogReader" /> and a base temperature. Counters below a minimal
        ///     value will be excluded from integration
        /// </summary>
        /// <param name="logReaders"></param>
        /// <param name="minSampleCount"></param>
        /// <param name="temperature"></param>
        /// <returns></returns>
        public IList<MmcfeEnergyState> CalculateEnergyStates(IReadOnlyList<MmcfeLogReader> logReaders, double temperature, long minSampleCount = 1)
        {
            if (!CheckLogReadersInEvaluationOrder(logReaders)) throw new InvalidOperationException("Readers are in wrong order for evaluation.");

            var result = new List<MmcfeEnergyState>(logReaders.Count);
            var (sampleSumIntegral, weightedSumIntegral, alphaDeltas) = (0.0, 0.0, GetAlphaDeltaList(logReaders));
            foreach (var (reader, logIndex) in logReaders.Select((x, i) => (x, i)))
            {
                var (header, routineParams) = (reader.EnergyHistogramReader.ReadHeader(), reader.ReadParameters());
                var (sampleSum, weightedSum, counterIndex) = (0L, 0.0, -1);

                foreach (var counter in reader.EnergyHistogramReader.ReadCounters())
                {
                    counterIndex++;
                    if (counter < minSampleCount) continue;
                    if (logIndex != 0) sampleSum += counter;
                    if (logIndex == logReaders.Count - 1) continue;

                    var energy = (header.MinValue + counterIndex * header.Stepping) * 0.5;
                    var expFactor = Math.Exp(energy * alphaDeltas[logIndex] / (temperature * Equations.Constants.BlotzmannEv));
                    weightedSum += counter * expFactor;
                }

                if (double.IsNaN(weightedSum) || double.IsInfinity(weightedSum))
                    throw new InvalidOperationException("Calculation is numerically unstable.");

                sampleSumIntegral += sampleSum == 0 ? 0 : Math.Log(sampleSum, Math.E);
                weightedSumIntegral += weightedSum < double.Epsilon ? 0 : Math.Log(weightedSum, Math.E);

                var innerEnergy = CalculateInnerEnergy(reader, minSampleCount);
                var freeEnergy = -temperature * Equations.Constants.BlotzmannEv * (sampleSumIntegral - weightedSumIntegral);
                result.Add(new MmcfeEnergyState(routineParams.AlphaCurrent, temperature / routineParams.AlphaCurrent, freeEnergy, innerEnergy));
            }

            return result;
        }

        /// <summary>
        ///     Checks if the provided <see cref="IReadOnlyList{T}" /> of <see cref="MmcfeLogReader" /> instances is ordered by
        ///     alpha (ascending)
        /// </summary>
        /// <param name="logReaders"></param>
        /// <returns></returns>
        public bool CheckLogReadersInEvaluationOrder(IReadOnlyList<MmcfeLogReader> logReaders)
        {
            for (var i = 1; i < logReaders.Count; i++)
            {
                if (logReaders[i].ReadParameters().AlphaCurrent <= logReaders[i - 1].ReadParameters().AlphaCurrent)
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Get a list of the alpha delta values of a sorted <see cref="IReadOnlyList{T}" /> of <see cref="MmcfeLogReader" />
        ///     instances
        /// </summary>
        /// <param name="logReaders"></param>
        /// <returns></returns>
        public IList<double> GetAlphaDeltaList(IReadOnlyList<MmcfeLogReader> logReaders)
        {
            var result = new List<double>(logReaders.Count - 1);
            for (var i = 1; i < logReaders.Count; i++)
            {
                var delta = logReaders[i].ReadParameters().AlphaCurrent - logReaders[i - 1].ReadParameters().AlphaCurrent;
                result.Add(delta);
            }

            return result;
        }

        /// <summary>
        ///     Calculates the average inner energy due to interactions described by an <see cref="MmcfeLogReader" />
        /// </summary>
        /// <param name="logReader"></param>
        /// <param name="minCounter"></param>
        /// <returns></returns>
        public double CalculateInnerEnergy(MmcfeLogReader logReader, long minCounter = 1)
        {
            var (countSum, energySum, index, header) = (0L, 0.0, -1, logReader.EnergyHistogramReader.ReadHeader());

            foreach (var counter in logReader.EnergyHistogramReader.ReadCounters())
            {
                index++;
                if (counter < minCounter) continue;
                countSum += counter;
                energySum += 0.5 * counter * (header.MinValue + index * header.Stepping);
            }

            return energySum / countSum;
        }

        /// <summary>
        ///     Linear interpolation of an <see cref="MmcfeEnergyState" /> for a target temperature from a list of known states.
        ///     States have to be sorted by ascending alpha value for the function to work correctly
        /// </summary>
        /// <param name="energyStates"></param>
        /// <param name="targetTemperature"></param>
        /// <returns></returns>
        public MmcfeEnergyState LinearInterpolateEnergyState(IList<MmcfeEnergyState> energyStates, double targetTemperature)
        {
            // Note: By default the list is sorted by alpha, and T is ~ 1 / alpha => Do the comparison inverse for the binary search to work
            var index = energyStates.CppLowerBound(new MmcfeEnergyState(0, targetTemperature, 0, 0),
                Comparer<MmcfeEnergyState>.Create((a, b) => b.Temperature.CompareTo(a.Temperature)));

            if (index == 0) index = 1;
            if (index >= energyStates.Count) index = energyStates.Count - 1;
            var (lhs, rhs) = (energyStates[index - 1], energyStates[index]);

            var factor = (targetTemperature - lhs.Temperature) / (rhs.Temperature - lhs.Temperature);
            var alpha = lhs.Alpha + (rhs.Alpha - lhs.Alpha) * factor;
            var energyU = lhs.InnerEnergy + (rhs.InnerEnergy - lhs.InnerEnergy) * factor;
            var energyF = lhs.FreeEnergy + (rhs.FreeEnergy - lhs.FreeEnergy) * factor;
            return new MmcfeEnergyState(alpha, targetTemperature, energyF, energyU);
        }

        /// <summary>
        ///     Calculates an average and deviation from an <see cref="IEnumerable{T}" /> of <see cref="MmcfeEnergyState" /> items (Temperature is excluded)
        /// </summary>
        /// <param name="energyStates"></param>
        /// <returns></returns>
        public (MmcfeEnergyState Average, MmcfeEnergyState Error) AverageWithSem(IEnumerable<MmcfeEnergyState> energyStates)
        {
            if (!(energyStates is IReadOnlyList<MmcfeEnergyState> list)) list = energyStates.ToList();
            var alpha = Equations.Statistics.AverageWithSem(list, x => x.Alpha);
            var freeEnergy = Equations.Statistics.AverageWithSem(list, x => x.FreeEnergy);
            var innerEnergy = Equations.Statistics.AverageWithSem(list, x => x.InnerEnergy);
            var temperature = Equations.Statistics.AverageWithSem(list, x => x.Temperature);
            var entropy = Equations.Statistics.AverageWithSem(list, x => x.Entropy);

            var average = new MmcfeEnergyState(alpha.Average, temperature.Average, freeEnergy.Average, innerEnergy.Average, entropy.Average);
            var error = new MmcfeEnergyState(alpha.Error, temperature.Error, freeEnergy.Error, innerEnergy.Error, entropy.Error);
            return (average, error);
        }

        /// <summary>
        ///     Calculates a new <see cref="MmcfeEnergyState" /> that represents the delta to a reference (Optional defect count to
        ///     create a "by defect" result)
        /// </summary>
        /// <param name="state"></param>
        /// <param name="reference"></param>
        /// <param name="defectCount"></param>
        /// <returns></returns>
        public MmcfeEnergyState CalculateDeltaEnergyState(in MmcfeEnergyState state, in MmcfeEnergyState reference, int defectCount = 1)
        {
            var innerEnergy = (state.InnerEnergy - reference.InnerEnergy) / defectCount;
            var freeEnergy = (state.FreeEnergy - reference.FreeEnergy) / defectCount;
            return new MmcfeEnergyState(state.Alpha, state.Temperature, freeEnergy, innerEnergy);
        }
    }
}