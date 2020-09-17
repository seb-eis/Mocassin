using System.Runtime.InteropServices;

#pragma warning disable 1591

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
        public double ElectricFieldModulus { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double BaseFrequency { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double FixedNormalizationFactor { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int PreRunMcsp { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        private readonly int padding;
    }
}