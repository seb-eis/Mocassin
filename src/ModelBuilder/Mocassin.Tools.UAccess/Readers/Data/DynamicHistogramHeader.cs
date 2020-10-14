using System.Runtime.InteropServices;

namespace Mocassin.Tools.UAccess.Readers.Data
{
    /// <summary>
    ///     Header of a single dynamic size histogram struct of the 'C' Simulator
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 56, Pack = 8)]
    public readonly struct DynamicHistogramHeader
    {
        /// <summary>
        ///     The min value of the histogram (included)
        /// </summary>
        [field: MarshalAs(UnmanagedType.R8)]
        public double MinValue { get; }

        /// <summary>
        ///     The max value of the histogram (excluded)
        /// </summary>
        [field: MarshalAs(UnmanagedType.R8)]
        public double MaxValue { get; }

        /// <summary>
        ///     The sampling width of the counters
        /// </summary>
        [field: MarshalAs(UnmanagedType.R8)]
        public double Stepping { get; }

        /// <summary>
        ///     The sampling width inverse of the counters
        /// </summary>
        [field: MarshalAs(UnmanagedType.R8)]
        public double SteppingInverse { get; }

        /// <summary>
        ///     The counter for values above or equal to <see cref="MaxValue" />
        /// </summary>
        [field: MarshalAs(UnmanagedType.I8)]
        public long OverflowCount { get; }

        /// <summary>
        ///     The counter for values below <see cref="MinValue" />
        /// </summary>
        [field: MarshalAs(UnmanagedType.I8)]
        public long UnderflowCount { get; }

        /// <summary>
        ///     The number of entries in the histogram
        /// </summary>
        [field: MarshalAs(UnmanagedType.I8)]
        public long EntryCount { get; }
    }
}