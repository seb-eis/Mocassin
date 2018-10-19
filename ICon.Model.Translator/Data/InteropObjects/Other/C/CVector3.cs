using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation Vector 3 object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 24)]
    public struct CVector3
    {
        [field: MarshalAs(UnmanagedType.R8)]
        public double A { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double B { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double C { get; set; }
    }
}