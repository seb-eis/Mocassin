using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ICon.Model.Translator.Data
{
    /// <summary>
    /// C array access object. Placeholder for array access structs with 4 byte count field
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct C_ArrayAccess
    {
        [MarshalAs(UnmanagedType.I4)]
        private readonly int count;

        private readonly C_LongPtr start;

        private readonly C_LongPtr end;
    }
}
