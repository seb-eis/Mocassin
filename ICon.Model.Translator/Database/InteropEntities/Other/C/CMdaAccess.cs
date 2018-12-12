using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     C multidimensional array access object. Placeholder for array access structs with 3 long pointers
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 24)]
    public readonly struct CMdaAccess
    {
        private readonly CLongPtr header;

        private readonly CLongPtr start;

        private readonly CLongPtr end;
    }
}