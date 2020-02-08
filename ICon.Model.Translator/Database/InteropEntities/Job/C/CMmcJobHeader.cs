using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation job info object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct CMmcJobHeader
    {
        [field: MarshalAs(UnmanagedType.I8)]
        public long JobFlags { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double AbortTolerance { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int AbortSequenceLength { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int AbortSampleLength { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int AbortSampleInterval { get; set; }

        [MarshalAs(UnmanagedType.I4)] private readonly int paddingInt;
    }
}