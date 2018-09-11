using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Long pointer object. Placeholder for 8 byte pointers on unmanaged objects
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size =8)]
    public readonly struct C_LongPtr
    {
        [MarshalAs(UnmanagedType.I8)]
        private readonly long value;
    }
}
