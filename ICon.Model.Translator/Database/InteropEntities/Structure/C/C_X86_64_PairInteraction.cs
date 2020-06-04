using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation pair interaction definition. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 20)]
    public struct C_X86_64_PairInteraction
    {
        /// <summary>
        ///     The relative 4D vector to the target
        /// </summary>
        public C_X86_64_Vector4I RelativeVector { get; set; }

        /// <summary>
        ///     The affiliated energy table id of the interaction
        /// </summary>
        [field: MarshalAs(UnmanagedType.I4)]
        public int PairTableId { get; set; }
    }
}