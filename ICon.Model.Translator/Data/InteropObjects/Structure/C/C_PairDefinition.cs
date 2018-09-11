using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Simulation pair interaction definition. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct C_PairDefinition
    {
        private C_Vector4 relativeVector;

        [MarshalAs(UnmanagedType.I4)]
        private int trackerId;

        [MarshalAs(UnmanagedType.I8)]
        private readonly long longPadding;

        public int TrackerId { get => trackerId; set => trackerId = value; }

        public C_Vector4 RelativeVector { get => relativeVector; set => relativeVector = value; }
    }
}
