using System;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Represents a state pair of a donor and acceptor particle index
    /// </summary>
    public readonly struct StatePair : IComparable<StatePair>
    {
        /// <summary>
        ///     The status of the position the state pair belongs to
        /// </summary>
        public PositionStability PositionStability { get; }

        /// <summary>
        ///     The acceptor particle index
        /// </summary>
        public int DonorIndex { get; }

        /// <summary>
        ///     The donor particle index
        /// </summary>
        public int AcceptorIndex { get; }

        /// <summary>
        ///     Creates new state pair from donor and acceptor particle index and a position status
        /// </summary>
        /// <param name="donorIndex"></param>
        /// <param name="acceptorIndex"></param>
        /// <param name="positionStability"></param>
        public StatePair(int donorIndex, int acceptorIndex, PositionStability positionStability)
            : this()
        {
            DonorIndex = donorIndex;
            AcceptorIndex = acceptorIndex;
            PositionStability = positionStability;
        }

        /// <summary>
        ///     Get a state pair for an unstable position that is always 0 for the acceptor state
        /// </summary>
        /// <param name="donorIndex"></param>
        /// <returns></returns>
        public static StatePair MakeUnstable(int donorIndex)
        {
            return new StatePair(donorIndex, 0, PositionStability.Unstable);
        }

        /// <summary>
        ///     Creates for the correct position status, i.e. the acceptor state of non-stables will be corrected to 0
        /// </summary>
        /// <param name="donorIndex"></param>
        /// <param name="acceptorIndex"></param>
        /// <param name="positionStability"></param>
        /// <returns></returns>
        public static StatePair CreateForStatus(int donorIndex, int acceptorIndex, PositionStability positionStability)
        {
            return positionStability == PositionStability.Unstable
                ? MakeUnstable(donorIndex)
                : new StatePair(donorIndex, acceptorIndex, positionStability);
        }

        /// <summary>
        ///     Compares donor index than acceptor index
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(StatePair other)
        {
            var donorComp = DonorIndex.CompareTo(other.DonorIndex);
            return donorComp == 0
                ? AcceptorIndex.CompareTo(other.AcceptorIndex)
                : donorComp;
        }
    }
}