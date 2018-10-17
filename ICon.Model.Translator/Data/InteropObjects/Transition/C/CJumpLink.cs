using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Simulation jump link object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct CJumpLink
    {
        [MarshalAs(UnmanagedType.I4)]
        private int jumpPathId;

        [MarshalAs(UnmanagedType.I4)]
        private int linkId;

        public int JumpPathId { get => jumpPathId; set => jumpPathId = value; }

        public int LinkId { get => linkId; set => linkId = value; }
    }
}
