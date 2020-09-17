using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Long pointer object. Placeholder for 8 byte pointers on unmanaged objects
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public readonly struct CLongPtr
    {
        [MarshalAs(UnmanagedType.I8)]
        private readonly long value;
    }
}