using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Tools.Evaluation.Helper;
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
        ///     <see cref="IReadOnlyList{T}" /> of <see cref="MmcfeLogReader" /> and a base temperature using the number of unit
        ///     cells for normalization
        /// </summary>
        /// <param name="logReaders"></param>
        /// <param name="baseTemperature"></param>
        /// <param name="numberOfUnitCells"></param>
        /// <returns></returns>
        public IList<MmcfeEnergyState> CalculateEnergyStates(IReadOnlyList<MmcfeLogReader> logReaders, double baseTemperature, int numberOfUnitCells)
        {
            if (!CheckLogReadersInEvaluationOrder(logReaders)) throw new InvalidOperationException("Readers are in wrong order for evaluation.");
            if (numberOfUnitCells <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfUnitCells));

            var result = new List<MmcfeEnergyState>(logReaders.Count - 1);
            var dAlphas = CalculateAlphaDeltaList(logReaders);
            var sumOfLnMij = 0.0;

            // Normalizes the free energy calculation to one unit cell to prevent issues with exp(...) for very large absolute energy values
            var energyNormalization = 1.0 / numberOfUnitCells;

            // The general formalism is to replace the numerically unstable m'(Inf->T) calculation by a summation over LN(m'(low)/m'(high)) for all consecutive histogram pairs in range T=Inf -> T
            for (var i = 1; i < logReaders.Count; i++)
            {
                var dAlpha = dAlphas[i - 1];
                var highReader = logReaders[i - 1];
                var lowReader = logReaders[i];
                var lowHeader = lowReader.EnergyHistogramReader.ReadHeader();
                var lowParams = lowReader.ReadParameters();
                if (i == 1)
                {
                    var highInnerEnergy = CalculateInnerEnergy(highReader);
                    var highTempParams = highReader.ReadParameters();
                    result.Add(new MmcfeEnergyState(highTempParams.AlphaCurrent, baseTemperature / highTempParams.AlphaCurrent, 0, highInnerEnergy));
                }

                var nominator = 0.0;
                var denominator = 0.0;

                // The m_ij nominator in the Valleau & Card solution is the unweighted sum of samples of the higher temperature histogram
                foreach (var counter in highReader.EnergyHistogramReader.ReadCounters()) nominator += counter;

                // The m_ij denominator in the Valleau & Card solution is the energy weighted sum of the lower temperature histogram
                var index = -1;
                foreach (var counter in lowReader.EnergyHistogramReader.ReadCounters())
                {
                    index++;
                    var energy = lowHeader.MinValue + index * lowHeader.Stepping;
                    var weighting = Math.Exp(energy * dAlpha * energyNormalization / (baseTemperature * Equations.Constants.BlotzmannEv));
                    denominator += weighting * counter;
                }

                var lnOfMij = Math.Log(nominator, Math.E) - Math.Log(denominator, Math.E);
                if (double.IsNaN(lnOfMij) || double.IsInfinity(lnOfMij))
                    throw new InvalidOperationException("Calculation is numerically unstable.");

                // The free energy at each temperature is defined as: -k*T*sum(ln(m_ij))
                sumOfLnMij += lnOfMij;
                var temperature = baseTemperature / lowParams.AlphaCurrent;
                var innerEnergy = CalculateInnerEnergy(lowReader);
                var freeEnergy = -temperature * Equations.Constants.BlotzmannEv * sumOfLnMij / energyNormalization;
                result.Add(new MmcfeEnergyState(lowParams.AlphaCurrent, temperature, freeEnergy, innerEnergy));
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
                var lowerTemperatureParameters = logReaders[i].ReadParameters();
                var higherTemperatureParameters = logReaders[i - 1].ReadParameters();
                if (lowerTemperatureParameters.AlphaCurrent <= higherTemperatureParameters.AlphaCurrent) return false;
            }

            return true;
        }

        /// <summary>
        ///     Get a list of the alpha delta values of a sorted <see cref="IReadOnlyList{T}" /> of <see cref="MmcfeLogReader" />
        ///     instances
        /// </summary>
        /// <param name="logReaders"></param>
        /// <returns></returns>
        public IList<double> CalculateAlphaDeltaList(IReadOnlyList<MmcfeLogReader> logReaders)
        {
            var result = new List<double>(logReaders.Count - 1);
            for (var i = 1; i < logReaders.Count; i++)
            {
                var lowerTemperatureParameters = logReaders[i].ReadParameters();
                var higherTemperatureParameters = logReaders[i - 1].ReadParameters();
                var delta = lowerTemperatureParameters.AlphaCurrent - higherTemperatureParameters.AlphaCurrent;
                result.Add(delta);
            }

            return result;
        }

        /// <summary>
        ///     Calculates the average inner energy due to interactions described by an <see cref="MmcfeLogReader" />
        /// </summary>
        /// <param name="logReader"></param>
        /// <returns></returns>
        public double CalculateInnerEnergy(MmcfeLogReader logReader)
        {
            var count = 0L;
            var energy = 0.0;
            var index = -1;
            var header = logReader.EnergyHistogramReader.ReadHeader();

            foreach (var counter in logReader.EnergyHistogramReader.ReadCounters())
            {
                index++;
                count += counter;
                energy += counter * (header.MinValue + index * header.Stepping);
            }

            return energy / count;
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
            var index = energyStates.BinarySearchFirstNotLessThanValue(new MmcfeEnergyState(0, targetTemperature, 0, 0),
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
        ///     Calculates an average and deviation from an <see cref="IEnumerable{T}" /> of <see cref="MmcfeEnergyState" /> items
        ///     (Temperature is excluded)
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