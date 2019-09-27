using System;
using System.Runtime.InteropServices;
using Mocassin.Tools.UAccess.Readers.Base;
using Mocassin.Tools.UAccess.Readers.HistogramReader.Components;

namespace Mocassin.Tools.UAccess.Readers
{
    /// <summary>
    ///     Provides fast read only access to the unmanaged binary state raw variable size histograms of the 'C' Mocassin.Simulator
    /// </summary>
    /// <remarks>The access is context free and requires the affiliated model context for evaluation</remarks>
    public class DynamicHistogramReader
    {
        /// <summary>
        ///     Get the byte count of the <see cref="DynamicHistogramHeader"/>
        /// </summary>
        public static int HeaderByteCount { get; } = Marshal.SizeOf<DynamicHistogramHeader>();

        /// <summary>
        ///     Get the <see cref="BinaryStructureReader"/> for the binary representation
        /// </summary>
        private BinaryStructureReader BinaryReader { get; }

        /// <summary>
        ///     Creates a new <see cref="DynamicHistogramReader"/> using the provided <see cref="BinaryStructureReader"/>
        /// </summary>
        /// <param name="binaryReader"></param>
        private DynamicHistogramReader(BinaryStructureReader binaryReader)
        {
            BinaryReader = binaryReader ?? throw new ArgumentNullException(nameof(binaryReader));
        }

        /// <summary>
        ///     Reads the histogram header bytes as an <see cref="DynamicHistogramHeader"/> structure
        /// </summary>
        /// <returns></returns>
        public ref DynamicHistogramHeader ReadHeader()
        {
            return ref BinaryReader.ReadAs<DynamicHistogramHeader>(0);
        }

        /// <summary>
        ///     Reads the histogram counters as a <see cref="ReadOnlySpan{T}"/> of <see cref="long"/> values
        /// </summary>
        /// <returns></returns>
        public ReadOnlySpan<long> ReadCounters()
        {
            return BinaryReader.ReadAreaAs<long>(HeaderByteCount, BinaryReader.ByteCount);
        }

        /// <summary>
        ///     Creates a new <see cref="DynamicHistogramReader"/> using the provided <see cref="byte"/> array and checks for data consistency
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static DynamicHistogramReader Create(byte[] bytes)
        {
            var binaryReader = new BinaryStructureReader(bytes);
            var entryCount = binaryReader.ReadAs<DynamicHistogramHeader>(0).EntryCount;
            var byteCount = HeaderByteCount + entryCount * sizeof(long);
            if (byteCount != bytes.Length) throw new InvalidOperationException("Byte array has incorrect size.");
            return new DynamicHistogramReader(binaryReader);
        }
    }
}