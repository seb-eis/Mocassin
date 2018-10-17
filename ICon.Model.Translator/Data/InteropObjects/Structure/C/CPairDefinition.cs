using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Simulation pair interaction definition. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct CPairDefinition
    {
        private CVector4 relativeVector;

        [MarshalAs(UnmanagedType.I4)]
        private int tableId;

        [MarshalAs(UnmanagedType.I8)]
        private readonly long longPadding;

        public int TableId { get => tableId; set => tableId = value; }

        public CVector4 RelativeVector { get => relativeVector; set => relativeVector = value; }
    }
}
