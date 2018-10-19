using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation pair interaction definition. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct CPairDefinition
    {
        [MarshalAs(UnmanagedType.I8)]
        private readonly long longPadding;

        [field: MarshalAs(UnmanagedType.I4)]
        public int TableId { get; set; }

        public CVector4 RelativeVector { get; set; }
    }
}