using System.Runtime.InteropServices;

namespace ICon.Model.Translator
{
    /// <summary>
    /// C state code object. Layout marshals to its unmanaged 'C' counterpart
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct CStateCode
    {
        [MarshalAs(UnmanagedType.I8)]
        private long code;

        public long Code
        {
            get => code;
            set => code = value;
        }
    }
}