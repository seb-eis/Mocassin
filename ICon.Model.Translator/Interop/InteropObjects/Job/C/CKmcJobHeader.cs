using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation job info object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 40)]
    public struct CKmcJobHeader
    {
        [field: MarshalAs(UnmanagedType.I8)]
        public long JobFlags { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double FieldMagnitude { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double BaseFrequency { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double FixedNormFactor { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int DynamicTrackerCount { get; set; }

        [MarshalAs(UnmanagedType.I4)] 
        private readonly int paddingInt;
    }
}