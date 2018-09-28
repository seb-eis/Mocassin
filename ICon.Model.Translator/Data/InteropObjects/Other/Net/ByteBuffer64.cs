using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Interop object wrapper for the 64 bytes buffer type used in the unmanaged simulation
    /// </summary>
    public class ByteBuffer64 : InteropObject<CByteBuffer64>
    {
        /// <inheritdoc />
        public ByteBuffer64()
        {
        }

        /// <inheritdoc />
        public ByteBuffer64(CByteBuffer64 structure) : base(structure)
        {
        }
    }
}
