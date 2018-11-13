using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation lattice info object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 72)]
    public struct CLatticeInfo
    {
        private readonly CMdaAccess latticeAccess;

        private readonly CMdaAccess backgroundAccess;

        [MarshalAs(UnmanagedType.I8)]
        private readonly long paddingLong;

        public CVector4 SizeVector { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int NumberOfMobiles { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int NumberOfSelectAtoms { get; set; }
    }
}