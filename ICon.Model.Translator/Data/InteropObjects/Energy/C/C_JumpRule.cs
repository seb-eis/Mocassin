using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Jump rule simulation object Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 48)]
    public struct C_JumpRule
    {
        [MarshalAs(UnmanagedType.I8)]
        private long stateCode0;

        [MarshalAs(UnmanagedType.I8)]
        private long stateCode1;

        [MarshalAs(UnmanagedType.I8)]
        private long stateCode2;

        [MarshalAs(UnmanagedType.R8)]
        private double frequencyFactor;

        [MarshalAs(UnmanagedType.R8)]
        private double fieldFactor;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private byte[] trackerOrder;

        public long StateCode0 { get => stateCode0; set => stateCode0 = value; }

        public long StateCode1 { get => stateCode1; set => stateCode1 = value; }

        public long StateCode2 { get => stateCode2; set => stateCode2 = value; }

        public double FrequencyFactor { get => frequencyFactor; set => frequencyFactor = value; }

        public double FieldFactor { get => fieldFactor; set => fieldFactor = value; }

        public byte[] TrackerOrder
        {
            get => trackerOrder;
            set
            {
                if ((value?.Length ?? throw new ArgumentNullException()) != 8)
                {
                    throw new ArgumentException("Aray has to be of size 8");
                }
                trackerOrder = value;
            }
        }
    }
}
