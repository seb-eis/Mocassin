using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation pair interaction definition. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct CPairDefinition
    {
        public CVector4 RelativeVector { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int EnergyTableId { get; set; }

        [MarshalAs(UnmanagedType.I4)]
        private readonly int padding0;

        [MarshalAs(UnmanagedType.I8)]
        private readonly long padding1;
    }
}