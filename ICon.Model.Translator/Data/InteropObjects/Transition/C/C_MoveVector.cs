using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Simulation Vector 3 object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct C_MoveVector
    {
        [MarshalAs(UnmanagedType.I4)]
        private readonly int trackerId;

        private readonly C_Vector3 vector;

        public int TrackerId => trackerId;

        public C_Vector3 Vector => vector;
    }
}
