using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation Vector 3 object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct CMoveVector
    {
        [MarshalAs(UnmanagedType.I4)] 
        private readonly int paddingInt;

        public CVector3 Vector { get; set; }

        [field: MarshalAs(UnmanagedType.I4)]
        public int TrackerId { get; set; }
    }
}