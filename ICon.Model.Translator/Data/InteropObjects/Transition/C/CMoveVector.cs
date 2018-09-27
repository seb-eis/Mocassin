using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Simulation Vector 3 object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct CMoveVector
    {
        [MarshalAs(UnmanagedType.I4)]
        private int trackerId;

        [MarshalAs(UnmanagedType.I4)]
        private readonly int paddingInt;

        private readonly CVector3 vector;

        public CVector3 Vector => vector;

        public int TrackerId { get => trackerId; set => trackerId = value; }
    }
}
