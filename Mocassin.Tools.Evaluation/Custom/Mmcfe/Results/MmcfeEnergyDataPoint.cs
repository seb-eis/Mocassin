using System;
using System.Buffers.Text;
using System.Text.RegularExpressions;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe
{
    /// <summary>
    ///     Describes an energy hyper surface data point from a large collection average over a set of simulations with attached meta data
    /// </summary>
    public class MmcfeEnergyDataPoint
    {
        /// <summary>
        ///     Get the number of samples for the point
        /// </summary>
        public int SampleCount { get; }

        /// <summary>
        ///     Get the particles counts for each index
        /// </summary>
        public int[] ParticleCounts { get; }

        /// <summary>
        ///     Get the <see cref="MmcfeEnergyState"/> describing the data point
        /// </summary>
        public MmcfeEnergyState EnergyState { get; }

        /// <summary>
        ///     Get the <see cref="MmcfeEnergyState"/> describing standard deviation of the data point
        /// </summary>
        public MmcfeEnergyState EnergyStateError { get; }

        /// <summary>
        ///     Get the <see cref="MmcfeLogMetaEntry"/> describing the data point meta information
        /// </summary>
        public MmcfeLogMetaEntry MetaEntry { get; }

        /// <summary>
        ///     Creates a new <see cref="MmcfeEnergyDataPoint"/> that describes a result hyper surface data point
        /// </summary>
        /// <param name="sampleCount"></param>
        /// <param name="energyState"></param>
        /// <param name="energyStateError"></param>
        /// <param name="metaEntry"></param>
        public MmcfeEnergyDataPoint(int sampleCount, in MmcfeEnergyState energyState, in MmcfeEnergyState energyStateError, MmcfeLogMetaEntry metaEntry)
        {
            SampleCount = sampleCount;
            EnergyState = energyState;
            EnergyStateError = energyStateError;
            MetaEntry = metaEntry ?? throw new ArgumentNullException(nameof(metaEntry));
            ParticleCounts = ParseParticleCounts(metaEntry.ParticleCountInfo);
        }

        /// <summary>
        ///     Parses the particle count <see cref="string"/> into an integer array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int[] ParseParticleCounts(string str)
        {
            var raw = str.Split(',');
            var result = new int[raw.Length];
            for (var i = 0; i < raw.Length; i++)
            {
                result[i] = int.Parse(raw[i]);
            }

            return result;
        }
    }
}