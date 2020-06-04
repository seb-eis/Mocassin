using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     C multidimensional array access object. Placeholder for array access structs with 3 long pointers
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 24)]
    public readonly struct C_X86_64_RectangularArrayAccess
    {
        private readonly C_X86_64_Ptr header;

        private readonly C_X86_64_Ptr start;

        private readonly C_X86_64_Ptr end;
    }
}