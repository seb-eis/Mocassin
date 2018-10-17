using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Simulation interaction range object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct CInteractionRange
    {
        [MarshalAs(UnmanagedType.I4)]
        private int a;

        [MarshalAs(UnmanagedType.I4)]
        private int b;

        [MarshalAs(UnmanagedType.I4)]
        private int c;

        [MarshalAs(UnmanagedType.I4)]
        private readonly int paddingInt;

        public int A { get => a; set => a = value; }

        public int B { get => b; set => b = value; }

        public int C { get => c; set => c = value; }
    }
}
