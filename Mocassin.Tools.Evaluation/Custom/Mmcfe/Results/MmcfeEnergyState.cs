using System;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe
{
    /// <summary>
    ///     Describes an energy state of a single stage of an MMCFE simulation series
    /// </summary>
    public readonly struct MmcfeEnergyState
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
        ///     Creates a new <see cref="MmcfeEnergyState"/>
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
        }
    }
}