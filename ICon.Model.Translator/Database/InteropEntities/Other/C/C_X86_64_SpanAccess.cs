using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     C array/span access object. Placeholder for array access structs with 4 byte count field
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public readonly struct C_X86_64_SpanAccess
    {
        private readonly C_X86_64_Ptr start;

        private readonly C_X86_64_Ptr end;
    }
}