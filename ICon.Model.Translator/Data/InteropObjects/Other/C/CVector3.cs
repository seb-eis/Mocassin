using System.Runtime.InteropServices;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Simulation Vector 3 object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 24)]
    public struct CVector3
    {
        [MarshalAs(UnmanagedType.R8)]
        private double a;

        [MarshalAs(UnmanagedType.R8)]
        private double b;

        [MarshalAs(UnmanagedType.R8)]
        private double c;

        public double A { get => a; set => a = value; }

        public double B { get => b; set => b = value; }

        public double C { get => c; set => c = value; }
    }
}
