using System.Runtime.InteropServices;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Simulation Vector 4 object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct CVector4
    {
        [MarshalAs(UnmanagedType.I4)]
        private int a;

        [MarshalAs(UnmanagedType.I4)]
        private int b;

        [MarshalAs(UnmanagedType.I4)]
        private int c;

        [MarshalAs(UnmanagedType.I4)]
        private int d;

        public int A { get => a; set => a = value; }

        public int B { get => b; set => b = value; }

        public int C { get => c; set => c = value; }

        public int D { get => d; set => d = value; }
    }
}
