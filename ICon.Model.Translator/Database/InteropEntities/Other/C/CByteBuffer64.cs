using System;
using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     64 entry byte buffer. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 64)]
    public struct CByteBuffer64
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        private byte[] buffer;

        public byte[] Buffer
        {
            get => buffer;
            set
            {
                if ((value?.Length ?? throw new ArgumentNullException(nameof(value))) != 64)
                    throw new ArgumentException("Byte array is not of length 64", nameof(value));
                buffer = value;
            }
        }
    }
}