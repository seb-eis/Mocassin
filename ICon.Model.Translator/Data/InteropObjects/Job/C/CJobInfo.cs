using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Simulation job info object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 72)]
    public struct CJobInfo
    {
        [MarshalAs(UnmanagedType.I8)]
        private long jobFlags;

        [MarshalAs(UnmanagedType.I8)]
        private long statusFlags;

        [MarshalAs(UnmanagedType.I8)]
        private long stateSize;

        [MarshalAs(UnmanagedType.I8)]
        private long targetMcsp;

        [MarshalAs(UnmanagedType.I8)]
        private long timeLimit;

        [MarshalAs(UnmanagedType.R8)]
        private double temperature;

        [MarshalAs(UnmanagedType.R8)]
        private double minimalSuccessRate;

        private readonly CLongPtr jobHeaderPtr;

        [MarshalAs(UnmanagedType.I4)]
        private int objectId;

        [MarshalAs(UnmanagedType.I4)]
        private readonly int paddingInt;

        public int ObjectId { get => objectId; set => objectId = value; }

        public long JobFlags { get => jobFlags; set => jobFlags = value; }

        public long StatusFlags { get => statusFlags; set => statusFlags = value; }

        public long StateSize { get => stateSize; set => stateSize = value; }

        public long TargetMcsp { get => targetMcsp; set => targetMcsp = value; }

        public long TimeLimit { get => timeLimit; set => timeLimit = value; }

        public double Temperature { get => temperature; set => temperature = value; }

        public double MinimalSuccessRate { get => minimalSuccessRate; set => minimalSuccessRate = value; }
    }
}
