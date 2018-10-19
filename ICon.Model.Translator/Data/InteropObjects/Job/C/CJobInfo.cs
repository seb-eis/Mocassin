using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation job info object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 72)]
    public struct CJobInfo
    {
        private readonly CLongPtr jobHeaderPtr;

        [MarshalAs(UnmanagedType.I4)] 
        private readonly int paddingInt;

        [field: MarshalAs(UnmanagedType.I4)]
        public int ObjectId { get; set; }

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

        [field: MarshalAs(UnmanagedType.R8)]
        public double Temperature { get; set; }

        [field: MarshalAs(UnmanagedType.R8)]
        public double MinimalSuccessRate { get; set; }
    }
}