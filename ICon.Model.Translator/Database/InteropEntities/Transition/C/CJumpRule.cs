using System;
using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Jump rule simulation object Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 56)]
    public struct CJumpRule
    {
        [field: MarshalAs(UnmanagedType.I8)]
        public long StateCode0 { get; set; }

        [field: MarshalAs(UnmanagedType.I8)]
        public long StateCode1 { get; set; }

        [field: MarshalAs(UnmanagedType.I8)]
        public long StateCode2 { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double AttemptFrequencyFraction { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double ElectricFieldFactor { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double StaticVirtualJumpEnergyCorrection { get; set; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private byte[] trackerOrder;

        public byte[] TrackerOrder
        {
            get => trackerOrder;
            set
            {
                if ((value?.Length ?? throw new ArgumentNullException()) != 8)
                    throw new ArgumentException("Array has to be of size 8");

                trackerOrder = value;
            }
        }
    }
}