using System;
using System.IO;
using Mocassin.Tools.UAccess.Readers.Base;
using Mocassin.Tools.UAccess.Readers.McsReader.Components;

namespace Mocassin.Tools.UAccess.Readers
{
    /// <summary>
    ///     Provides fast read only access to the unmanaged binary state raw output 'mcs' data file of the 'C' Mocassin.Simulator
    /// </summary>
    /// <remarks>The access is context free and requires the affiliated model context for evaluation</remarks>
    public class McsContentReader
    {
        private readonly McsHeader _header;

        /// <summary>
        ///     Get the <see cref="BinaryReader" /> to access the byte array contents
        /// </summary>
        private BinaryStructureReader BinaryReader { get; }

        /// <summary>
        ///     Get a reference to the <see cref="McsHeader"/> read from the buffer at object creation
        /// </summary>
        public ref readonly McsHeader Header => ref _header;

        /// <summary>
        ///     Create a new <see cref="McsContentReader"/> that uses the passed <see cref="BinaryStructureReader"/>
        /// </summary>
        /// <param name="binaryStructureReader"></param>
        private McsContentReader(BinaryStructureReader binaryStructureReader)
        {
            BinaryReader = binaryStructureReader ?? throw new ArgumentNullException(nameof(binaryStructureReader));
            _header = GetHeader();
        }

        /// <summary>
        ///     Get a reference to the <see cref="McsHeader" /> directly from the buffer
        /// </summary>
        public ref McsHeader GetHeader()
        {
            return ref BinaryReader.ReadAs<McsHeader>(0);
        }

        /// <summary>
        ///     Get a reference to the <see cref="McsMetaData" /> that store the meta results
        /// </summary>
        public ref McsMetaData GetMetaData()
        {
            return ref BinaryReader.ReadAs<McsMetaData>(GetHeader().MetaDataOffset);
        }

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> of <see cref="byte" /> that represents the simulation lattice result
        /// </summary>
        public ReadOnlySpan<byte> GetLattice()
        {
            return BinaryReader.ReadArea<byte>(Header.LatticeDataOffset, Header.CounterDataOffset);
        }

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> of <see cref="McsCycleCounter" /> that store the species cycle results
        ///     occurence info
        /// </summary>
        public ReadOnlySpan<McsCycleCounter> GetCycleCounters()
        {
            return BinaryReader.ReadArea<McsCycleCounter>(Header.CounterDataOffset, Header.GlobalTrackerDataOffset);
        }

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> of <see cref="McsMovementTracker" /> that store the global tracking system
        ///     results
        /// </summary>
        public ReadOnlySpan<McsMovementTracker> GetGlobalMovementTrackers()
        {
            return BinaryReader.ReadArea<McsMovementTracker>(Header.GlobalTrackerDataOffset, Header.MobileTrackerDataOffset);
        }

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> of <see cref="McsMovementTracker" /> that store the mobile tracking system
        ///     results
        /// </summary>
        public ReadOnlySpan<McsMovementTracker> GetMobileMovementTrackers()
        {
            return BinaryReader.ReadArea<McsMovementTracker>(Header.MobileTrackerDataOffset, Header.StaticTrackerDataOffset);
        }

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> of <see cref="McsMovementTracker" /> that store the static tracking system
        ///     results
        /// </summary>
        public ReadOnlySpan<McsMovementTracker> GetStaticMovementTrackers()
        {
            return BinaryReader.ReadArea<McsMovementTracker>(Header.StaticTrackerDataOffset, Header.MobileTrackerIndexingDataOffset);
        }

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> of <see cref="int" /> that maps mobile tracker indices onto their affiliated
        ///     lattice position indices
        /// </summary>
        public ReadOnlySpan<int> GetMobileTrackerMapping()
        {
            return BinaryReader.ReadArea<int>(Header.MobileTrackerIndexingDataOffset, Header.JumpStatisticDataOffset);
        }

        /// <summary>
        ///     Get a <see cref="ReadOnlySpan{T}" /> of <see cref="McsJumpStatistic" /> that store the jump histogram system
        ///     results
        /// </summary>
        public ReadOnlySpan<McsJumpStatistic> GetJumpStatistics()
        {
            return BinaryReader.ReadArea<McsJumpStatistic>(Header.JumpStatisticDataOffset, BinaryReader.BinaryLength);
        }

        /// <summary>
        ///     Creates a new <see cref="McsContentReader" /> for the provided byte array
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static McsContentReader Create(byte[] bytes)
        {
            return new McsContentReader(new BinaryStructureReader(bytes));
        }

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