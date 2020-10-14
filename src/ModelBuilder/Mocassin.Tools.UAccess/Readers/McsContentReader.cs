using System;
using System.IO;
using Mocassin.Tools.UAccess.Readers.Data;

namespace Mocassin.Tools.UAccess.Readers
{
    /// <summary>
    ///     Provides fast read only access to the unmanaged binary state raw output 'mcs' data file of the 'C'
    ///     Mocassin.Simulator
    /// </summary>
    /// <remarks>The access is context free and requires the affiliated model context for evaluation</remarks>
    public class McsContentReader : IDisposable
    {
        private readonly McsHeader header;

        /// <summary>
        ///     Get the value that indicates if data is not set on the <see cref="McsHeader" />
        /// </summary>
        public static int DataNotPresentOffsetIndicator { get; } = -1;

        /// <summary>
        ///     Get the <see cref="BinaryReader" /> to access the byte array contents
        /// </summary>
        private BinaryStructureReader BinaryReader { get; }

        /// <summary>
        ///     Get a reference to the <see cref="McsHeader" /> read from the buffer at object creation
        /// </summary>
        public ref readonly McsHeader Header => ref header;

        /// <summary>
        ///     Get a boolean flag if the reader is reading an MMC file with reduced data
        /// </summary>
        public bool IsReadingMmcState { get; }

        /// <summary>
        ///     Create a new <see cref="McsContentReader" /> that uses the passed <see cref="BinaryStructureReader" />
        /// </summary>
        /// <param name="binaryStructureReader"></param>
        private McsContentReader(BinaryStructureReader binaryStructureReader)
        {
            BinaryReader = binaryStructureReader ?? throw new ArgumentNullException(nameof(binaryStructureReader));
            header = ReadHeader();
            IsReadingMmcState = header.MobileTrackerOffset < 0;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            BinaryReader.Dispose();
        }

        /// <summary>
        ///     Get a reference to the <see cref="McsHeader" /> directly from the buffer
        /// </summary>
        public ref McsHeader ReadHeader() => ref BinaryReader.ReadAs<McsHeader>(0);

        /// <summary>
        ///     Get a reference to the <see cref="McsMetaData" /> that store the meta results
        /// </summary>
        public ref McsMetaData ReadMetaData() => ref BinaryReader.ReadAs<McsMetaData>(ReadHeader().MetaOffset);

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> of <see cref="byte" /> that represents the simulation lattice result
        /// </summary>
        public ReadOnlySpan<byte> ReadLattice() => BinaryReader.ReadAreaAs<byte>(Header.LatticeOffset, Header.CountersOffset);

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> of <see cref="McsCycleCounter" /> that store the species cycle results
        ///     occurence info
        /// </summary>
        public ReadOnlySpan<McsCycleCounter> ReadCycleCounters() => BinaryReader.ReadAreaAs<McsCycleCounter>(Header.CountersOffset,
            IsReadingMmcState ? BinaryReader.ByteCount : Header.GlobalTrackerOffset);

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> of <see cref="McsMovementTracker" /> that store the global tracking system
        ///     results
        /// </summary>
        public ReadOnlySpan<McsMovementTracker> ReadGlobalTrackers() =>
            IsReadingMmcState
                ? ReadOnlySpan<McsMovementTracker>.Empty
                : BinaryReader.ReadAreaAs<McsMovementTracker>(Header.GlobalTrackerOffset, Header.MobileTrackerOffset);

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> of <see cref="McsMovementTracker" /> that store the mobile tracking system
        ///     results
        /// </summary>
        public ReadOnlySpan<McsMovementTracker> ReadMobileTrackers() =>
            IsReadingMmcState
                ? ReadOnlySpan<McsMovementTracker>.Empty
                : BinaryReader.ReadAreaAs<McsMovementTracker>(Header.MobileTrackerOffset, Header.StaticTrackerOffset);

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> of <see cref="McsMovementTracker" /> that store the static tracking system
        ///     results
        /// </summary>
        public ReadOnlySpan<McsMovementTracker> ReadStaticTrackers() =>
            IsReadingMmcState
                ? ReadOnlySpan<McsMovementTracker>.Empty
                : BinaryReader.ReadAreaAs<McsMovementTracker>(Header.StaticTrackerOffset, Header.MobileTrackerIndexingOffset);

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> of <see cref="int" /> that maps mobile tracker indices onto their affiliated
        ///     lattice position indices
        /// </summary>
        public ReadOnlySpan<int> ReadMobileTrackerMapping() =>
            IsReadingMmcState
                ? ReadOnlySpan<int>.Empty
                : BinaryReader.ReadAreaAs<int>(Header.MobileTrackerIndexingOffset, Header.JumpStatisticsOffset);

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> of <see cref="McsJumpStatistic" /> that store the jump histogram system
        ///     results
        /// </summary>
        public ReadOnlySpan<McsJumpStatistic> ReadJumpStatistics()
        {
            if (Header.JumpStatisticsOffset == DataNotPresentOffsetIndicator) return ReadOnlySpan<McsJumpStatistic>.Empty;
            return IsReadingMmcState
                ? ReadOnlySpan<McsJumpStatistic>.Empty
                : BinaryReader.ReadAreaAs<McsJumpStatistic>(Header.JumpStatisticsOffset, BinaryReader.ByteCount);
        }

        /// <summary>
        ///     Creates a new <see cref="McsContentReader" /> for the provided byte array
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static McsContentReader Create(byte[] bytes) => new McsContentReader(new BinaryStructureReader(bytes));

        /// <summary>
        ///     Creates a new <see cref="McsContentReader" /> for the provided filepath
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static McsContentReader Create(string filename)
        {
            if (!File.Exists(filename)) throw new ArgumentException("File does not exist.", nameof(filename));
            var bytes = File.ReadAllBytes(filename);

            if (bytes.Length == 0) throw new ArgumentException("File is empty.", nameof(filename));
            return Create(bytes);
        }
    }
}