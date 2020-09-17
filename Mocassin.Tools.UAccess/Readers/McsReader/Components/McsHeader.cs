using System.Runtime.InteropServices;

namespace Mocassin.Tools.UAccess.Readers.McsReader.Components
{
    /// <summary>
    ///     Simulation state header struct that provides build information about a 'C' Simulator state file
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 56, Pack = 8)]
    public readonly struct McsHeader
    {
        /// <summary>
        ///     Get the number of performed monte carlo steps
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public readonly long McsCount;

        /// <summary>
        ///     Get the number of performed simulation cycles
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public readonly long CycleCount;

        /// <summary>
        ///     Get set simulation flags
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public readonly ulong Flags;

        /// <summary>
        ///     Get the byte offset for the start of the meta data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int MetaOffset;

        /// <summary>
        ///     Get the byte offset for the start of the lattice data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int LatticeOffset;

        /// <summary>
        ///     Get the byte offset for the start of the counter data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int CountersOffset;

        /// <summary>
        ///     Get the byte offset for the start of the global tracker data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int GlobalTrackerOffset;

        /// <summary>
        ///     Get the byte offset for the start of the mobile tracker data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int MobileTrackerOffset;

        /// <summary>
        ///     Get the byte offset for the start of the static tracker data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int StaticTrackerOffset;

        /// <summary>
        ///     Get the byte offset for the start of the mobile tracker indexing data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int MobileTrackerIndexingOffset;

        /// <summary>
        ///     Get the byte offset for the start of the jump statistics data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int JumpStatisticsOffset;
    }
}