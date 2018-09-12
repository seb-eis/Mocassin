using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// C multidimensional array access object. Placeholder for array access structs with 4 byte count field
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 24)]
    public readonly struct C_MdaAccess
    {
        private readonly C_LongPtr header;

        private readonly C_LongPtr start;

        private readonly C_LongPtr end;
    }
}
