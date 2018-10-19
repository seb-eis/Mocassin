using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation jump link object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct CJumpLink
    {
        [field: MarshalAs(UnmanagedType.I4)]
        public int JumpPathId { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int LinkId { get; set; }
    }
}