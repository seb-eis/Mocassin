using System.Runtime.InteropServices;
using Mocassin.Mathematics.ValueTypes;
#pragma warning disable 1591

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation Vector 4 integer struct. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct C_X86_64_Vector4I
    {
        public C_X86_64_Vector4I(int a, int b, int c, int d)
            : this()
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public C_X86_64_Vector4I(in Vector4I vector4D)
            : this(vector4D.A, vector4D.B, vector4D.C, vector4D.P)
        {
        }

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