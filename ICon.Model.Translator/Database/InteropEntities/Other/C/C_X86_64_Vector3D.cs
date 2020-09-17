using System.Runtime.InteropServices;
using Mocassin.Mathematics.ValueTypes;

#pragma warning disable 1591

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation Vector 3 double struct. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 24)]
    public struct C_X86_64_Vector3D
    {
        [field: MarshalAs(UnmanagedType.R8)]
        public double A { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double B { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double C { get; set; }


        public C_X86_64_Vector3D(IVector3D vector)
            : this(vector.Coordinates.A, vector.Coordinates.B, vector.Coordinates.C)
        {
        }

        public C_X86_64_Vector3D(double a, double b, double c)
            : this()
        {
            A = a;
            B = b;
            C = c;
        }
    }
}