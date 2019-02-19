using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation interaction range object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct CInteractionRange
    {
        /// <summary>
        ///     The interaction range as a <see cref="CVector4" /> object
        /// </summary>
        public CVector4 Vector { get; set; }
    }
}