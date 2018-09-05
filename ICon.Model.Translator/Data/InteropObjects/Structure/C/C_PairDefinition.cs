using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Simulation pair interaction definition. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct C_PairDefinition
    {
        [MarshalAs(UnmanagedType.I4)]
        private int trackerId;

        private C_Vector4 relativeVector;

        public int TrackerId { get => trackerId; set => trackerId = value; }

        public C_Vector4 RelativeVector { get => relativeVector; set => relativeVector = value; }
    }
}
