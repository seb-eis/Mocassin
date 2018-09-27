using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Simulation job info object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 40)]
    public struct CKmcJobHeader
    {
        [MarshalAs(UnmanagedType.I8)]
        private long jobFalgs;

        [MarshalAs(UnmanagedType.R8)]
        double fieldMagnitude;

        [MarshalAs(UnmanagedType.R8)]
        double baseFrequency;

        [MarshalAs(UnmanagedType.R8)]
        double fixedNormFactor;

        [MarshalAs(UnmanagedType.I4)]
        private int dynamicTrackerCount;

        [MarshalAs(UnmanagedType.I4)]
        private readonly int paddingInt;

        public long JobFalgs { get => jobFalgs; set => jobFalgs = value; }

        public int DynamicTrackerCount { get => dynamicTrackerCount; set => dynamicTrackerCount = value; }

        public double FieldMagnitude { get => fieldMagnitude; set => fieldMagnitude = value; }

        public double BaseFrequency { get => baseFrequency; set => baseFrequency = value; }

        public double FixedNormFactor { get => fixedNormFactor; set => fixedNormFactor = value; }
    }
}
