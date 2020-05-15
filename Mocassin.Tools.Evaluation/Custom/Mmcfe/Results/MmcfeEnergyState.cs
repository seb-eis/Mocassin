using System;
using System.Globalization;
using System.Text;
using Mocassin.Tools.Evaluation.Queries.Base;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe
{
    /// <summary>
    ///     Describes an energy state of a single stage of an MMCFE simulation series
    /// </summary>
    public class MmcfeEnergyState
    {
        /// <summary>
        ///     Get the alpha value the sate belongs to
        /// </summary>
        public double Alpha { get; }

        /// <summary>
        ///     Get the equivalent temperature in [K]
        /// </summary>
        public double Temperature { get; }

        /// <summary>
        ///     Get the free energy due to interactions in [eV] (T = inf [K] is the reference point)
        /// </summary>
        public double FreeEnergy { get; }

        /// <summary>
        ///     Get the inner energy due to interactions in [eV]
        /// </summary>
        public double InnerEnergy { get; }

        /// <summary>
        ///     Get the entropy value resulting from free energy and inner energy under assumption of constant volume in [eV / K]
        /// </summary>
        public double Entropy { get; }

        /// <summary>
        ///     Creates a new <see cref="MmcfeEnergyState" /> with implicit entropy specification
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="temperature"></param>
        /// <param name="freeEnergy"></param>
        /// <param name="innerEnergy"></param>
        public MmcfeEnergyState(double alpha, double temperature, double freeEnergy, double innerEnergy)
        {
            Alpha = alpha;
            Temperature = temperature;
            FreeEnergy = freeEnergy;
            InnerEnergy = innerEnergy;
            Entropy = (InnerEnergy - FreeEnergy) / Temperature;
        }

        /// <summary>
        ///     Creates a new <see cref="MmcfeEnergyState" /> with explicit entropy specification
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="temperature"></param>
        /// <param name="freeEnergy"></param>
        /// <param name="innerEnergy"></param>
        /// <param name="entropy"></param>
        public MmcfeEnergyState(double alpha, double temperature, double freeEnergy, double innerEnergy, double entropy)
        {
            Alpha = alpha;
            Temperature = temperature;
            FreeEnergy = freeEnergy;
            InnerEnergy = innerEnergy;
            Entropy = entropy;
        }

        /// <summary>
        ///     Get a by defect result <see cref="MmcfeEnergyState" /> for the given defect count
        /// </summary>
        /// <param name="defectCount"></param>
        /// <returns></returns>
        public MmcfeEnergyState AsPerDefect(int defectCount)
        {
            return new MmcfeEnergyState(Alpha, Temperature, FreeEnergy / defectCount, InnerEnergy / defectCount, Entropy / defectCount);
        }

        /// <summary>
        ///     Get a <see cref="MmcfeEnergyState" /> where the energy values are relative to the provided reference
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public MmcfeEnergyState AsRelative(in MmcfeEnergyState reference)
        {
            return new MmcfeEnergyState(Alpha, Temperature, FreeEnergy - reference.FreeEnergy, InnerEnergy - reference.InnerEnergy,
                Entropy - reference.Entropy);
        }

        /// <summary>
        ///     Get a formatted string of the energy values in the order "InnerEnergy", "FreeEnergy" and "Entropy"
        /// </summary>
        /// <param name="entropyInKb"></param>
        /// <param name="formatProvider"></param>
        /// <param name="separator"></param>
        /// <param name="doubleFormat"></param>
        /// <returns></returns>
        public string GetFormattedEnergies(bool entropyInKb = true, IFormatProvider formatProvider = null, string separator = " ", string doubleFormat = "e13")
        {
            formatProvider ??= CultureInfo.InvariantCulture;
            var builder = new StringBuilder(100);
            builder.Append(InnerEnergy.ToString(doubleFormat, formatProvider));
            builder.Append(separator);
            builder.Append(FreeEnergy.ToString(doubleFormat, formatProvider));
            builder.Append(separator);
            var entropy = entropyInKb ? Entropy / Equations.Constants.BlotzmannEv : Entropy;
            builder.Append(entropy.ToString(doubleFormat, formatProvider));
            return builder.ToString();
        }
    }
}