using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ICon.Model.Translator.Data
{
    /// <summary>
    /// Short C array access object. Placeholder for array access structs with 1 byte count field
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct C_ShortArrayAccess
    {
        [MarshalAs(UnmanagedType.U1)]
        private readonly byte count;

        private readonly C_LongPtr start;

        private readonly C_LongPtr end;
    }
}
