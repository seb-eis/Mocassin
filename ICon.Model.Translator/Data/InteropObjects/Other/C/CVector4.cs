using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation Vector 4 object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct CVector4
    {
        [field: MarshalAs(UnmanagedType.I4)]
        public int A { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int B { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int C { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int D { get; set; }
    }
}