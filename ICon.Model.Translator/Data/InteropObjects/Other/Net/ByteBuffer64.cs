﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Interop object wrapper for the 64 bytes buffer type used in the unmanaged simulation
    /// </summary>
    public class ByteBuffer64 : InteropObjectBase<C_ByteBuffer64>
    {
        public ByteBuffer64()
        {
        }

        public ByteBuffer64(C_ByteBuffer64 structure) : base(structure)
        {
        }
    }
}
