using System.Runtime.InteropServices;
#pragma warning disable 1591

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation job info object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 88)]
    public struct CJobInfo
    {
        [field: MarshalAs(UnmanagedType.I8)]
        public long JobFlags { get; set; }

        [field: MarshalAs(UnmanagedType.I8)]
        public long StatusFlags { get; set; }

        [field: MarshalAs(UnmanagedType.I8)]
        public long StateSize { get; set; }

        [field: MarshalAs(UnmanagedType.I8)]
        public long TargetMcsp { get; set; }

        [field: MarshalAs(UnmanagedType.I8)]
        public long TimeLimit { get; set; }

        [field: MarshalAs(UnmanagedType.I8)]
        public long RngStateSeed { get; set; }

        [field: MarshalAs(UnmanagedType.I8)]
        public long RngIncreaseSeed { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double Temperature { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double MinimalSuccessRate { get; set; }

        private readonly CLongPtr jobHeaderPtr;

        [field: MarshalAs(UnmanagedType.I4)]
        public int ObjectId { get; set; }

        [MarshalAs(UnmanagedType.I4)] private readonly int paddingInt;
    }
}