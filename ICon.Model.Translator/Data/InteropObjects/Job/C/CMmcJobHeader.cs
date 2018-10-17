using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Simulation job info object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct CMmcJobHeader
    {
        [MarshalAs(UnmanagedType.I8)]
        private long jobFlags;

        [MarshalAs(UnmanagedType.R8)]
        private double abortTolerance;

        [MarshalAs(UnmanagedType.I4)]
        private int abortSequenceLength;

        [MarshalAs(UnmanagedType.I4)]
        private int abortSampleLEngth;

        [MarshalAs(UnmanagedType.I4)]
        private int abortSampleInterval;

        [MarshalAs(UnmanagedType.I4)]
        private readonly int paddingInt;

        public long JobFlags { get => jobFlags; set => jobFlags = value; }

        public double AbortTolerance { get => abortTolerance; set => abortTolerance = value; }

        public int AbortSequenceLength { get => abortSequenceLength; set => abortSequenceLength = value; }

        public int AbortSampleLEngth { get => abortSampleLEngth; set => abortSampleLEngth = value; }

        public int AbortSampleInterval { get => abortSampleInterval; set => abortSampleInterval = value; }
    }
}
