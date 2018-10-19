using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     C array access object. Placeholder for array access structs with 4 byte count field
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public readonly struct CArrayAccess
    {
        private readonly CLongPtr start;

        private readonly CLongPtr end;
    }
}