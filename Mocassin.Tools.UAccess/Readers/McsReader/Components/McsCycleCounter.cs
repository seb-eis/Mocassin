﻿using System.Runtime.InteropServices;

namespace Mocassin.Tools.UAccess.Readers.McsReader.Components
{
    /// <summary>
    ///     Simulation state counter collection struct that contains cycle counters of a single species of a 'C' Simulator
    ///     state file
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 48, Pack = 8)]
    public readonly struct McsCycleCounter
    {
        /// <summary>
        ///     Get the total number of cycles
        /// </summary>
        [MarshalAs(UnmanagedType.I8)] public readonly long CycleCount;

        /// <summary>
        ///     Get the number of successful cycles
        /// </summary>
        [MarshalAs(UnmanagedType.I8)] public readonly long SuccessCount;

        /// <summary>
        ///     Get the number of rejected cycles
        /// </summary>
        [MarshalAs(UnmanagedType.I8)] public readonly long RejectionCount;

        /// <summary>
        ///     Get the number of site blocked cycles
        /// </summary>
        [MarshalAs(UnmanagedType.I8)] public readonly long SiteBlockingCount;

        /// <summary>
        ///     Get the number of cycles with an energetically unstable start state
        /// </summary>
        [MarshalAs(UnmanagedType.I8)] public readonly long StartStateUnstableCount;

        /// <summary>
        ///     Get the number of cycles with an energetically unstable end state
        /// </summary>
        [MarshalAs(UnmanagedType.I8)] public readonly long EndStateUnstableCount;
    }
}