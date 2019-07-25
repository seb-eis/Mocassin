using System;
using System.Runtime.InteropServices;

namespace Mocassin.Tools.UAccess.Readers.McsReader.Components
{
    /// <summary>
    ///     Simulation jump histogram struct that contains the jump occurence sampling. Layout 'C' Simulator state file
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 8040, Pack = 8)]
    public unsafe struct McsJumpHistogram
    {
        /// <summary>
        ///     Get the minimal tracked energy value (Included)
        /// </summary>
        [MarshalAs(UnmanagedType.R8)] public readonly double MinEnergyValue;

        /// <summary>
        ///     Get the maximal tracked energy value (Excluded)
        /// </summary>
        [MarshalAs(UnmanagedType.R8)] public readonly double MaxEnergyValue;

        /// <summary>
        ///     Get the tracked energy sample stepping
        /// </summary>
        [MarshalAs(UnmanagedType.R8)] public readonly double Stepping;

        /// <summary>
        ///     Get the counter for energy occurrences that where above or equal to the max energy value
        /// </summary>
        [MarshalAs(UnmanagedType.I8)] public readonly long AboveMaxCount;

        /// <summary>
        ///     Get the counter for energy occurrences that where below the min energy value
        /// </summary>
        [MarshalAs(UnmanagedType.I8)] public readonly long BelowMinCount;

        /// <summary>
        ///     Fixed histogram buffer that stores the occurence counters
        /// </summary>
        private fixed long _histogramBuffer[1000];

        /// <summary>
        ///     Get a counter entry from the fixed counter buffer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public long this[int index] => _histogramBuffer[index];

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> to the counters
        /// </summary>
        public ReadOnlySpan<long> Counters
        {
            get
            {
                fixed (void* ptr = _histogramBuffer)
                {
                    return new ReadOnlySpan<long>(ptr, 1000);
                }
            }
        }
    }
}