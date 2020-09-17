using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation lattice info object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 24)]
    public struct C_X86_64_LatticeInfo
    {
        public C_X86_64_Vector4I SizeVector { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int NumberOfMobiles { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int NumberOfSelectAtoms { get; set; }
    }
}