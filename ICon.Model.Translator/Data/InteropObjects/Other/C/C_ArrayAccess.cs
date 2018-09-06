using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// C array access object. Placeholder for array access structs with 4 byte count field
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct C_ArrayAccess
    {
        [MarshalAs(UnmanagedType.I4)]
        private int count;

        private readonly C_LongPtr start;

        private readonly C_LongPtr end;

        public int Count { get => count; set => count = value; }
    }
}
