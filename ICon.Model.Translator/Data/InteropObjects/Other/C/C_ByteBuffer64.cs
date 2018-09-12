﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ICon.Model.Translator
{
    /// <summary>
    /// 64 entry byte buffer. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 64)]
    public struct C_ByteBuffer64
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        private byte[] buffer;

        public byte[] Buffer
        {
            get => buffer;
            set
            {
                if ((value?.Length ?? throw new ArgumentNullException(nameof(value))) != 64)
                {
                    throw new ArgumentException("Byte array is not of length 64", nameof(value));
                }
                buffer = value;
            }
        }
    }
}
