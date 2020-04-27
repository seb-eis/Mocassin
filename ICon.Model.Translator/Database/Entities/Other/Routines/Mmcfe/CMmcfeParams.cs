using System.Runtime.InteropServices;
#pragma warning disable 1591

namespace Mocassin.Model.Translator.Routines
{
    /// <summary>
    ///     Mmc free energy routine parameter data object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 56)]
    public struct CMmcfeParams
    {
        [field: MarshalAs(UnmanagedType.I4)]
        public int HistogramSize { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int AlphaCount { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double AlphaMin { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double AlphaMax { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double AlphaCurrent { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double HistogramRange { get; set; }

        [field: MarshalAs(UnmanagedType.I8)]
        public long RelaxPhaseCycleCount { get; set; }

        [field: MarshalAs(UnmanagedType.I8)]
        public long LogPhaseCycleCount { get; set; }
    }
}