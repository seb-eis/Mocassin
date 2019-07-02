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
        [MarshalAs(UnmanagedType.I4)]
        public readonly int Flags;

        /// <summary>
        ///     Get the byte offset for the start of the meta data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int MetaDataOffset;

        /// <summary>
        ///     Get the byte offset for the start of the lattice data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int LatticeDataOffset;

        /// <summary>
        ///     Get the byte offset for the start of the counter data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int CounterDataOffset;

        /// <summary>
        ///     Get the byte offset for the start of the global tracker data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int GlobalTrackerDataOffset;

        /// <summary>
        ///     Get the byte offset for the start of the mobile tracker data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int MobileTrackerDataOffset;

        /// <summary>
        ///     Get the byte offset for the start of the static tracker data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int StaticTrackerDataOffset;

        /// <summary>
        ///     Get the byte offset for the start of the mobile tracker indexing data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int MobileTrackerIndexingDataOffset;

        /// <summary>
        ///     Get the byte offset for the start of the jump statistics data block
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int JumpStatisticDataOffset;

        /// <summary>
        ///     Explicit padding to 56 bytes
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public readonly int _padding;
    }
}