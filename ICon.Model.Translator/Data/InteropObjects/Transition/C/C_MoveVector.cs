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
    public struct C_MoveVector
    {
        [MarshalAs(UnmanagedType.I4)]
        private int trackerId;

        [MarshalAs(UnmanagedType.I4)]
        private readonly int paddingInt;

        private readonly C_Vector3 vector;

        public C_Vector3 Vector => vector;

        public int TrackerId { get => trackerId; set => trackerId = value; }
    }
}
