using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation interaction range object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct C_X86_64_InteractionRange
    {
        /// <summary>
        ///     The interaction range as a <see cref="C_X86_64_Vector4I" /> object
        /// </summary>
        public C_X86_64_Vector4I Vector { get; set; }
    }
}