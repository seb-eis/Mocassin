﻿using System;
using ICon.Model.Structures;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Represents a state pair of a donor and acceptor particle index
    /// </summary>
    public readonly struct StatePair
    {
        /// <summary>
        /// The status of the position the state pair belongs to
        /// </summary>
        public PositionStatus PositionStatus { get; }

        /// <summary>
        /// The acceptor particle index
        /// </summary>
        public int DonorIndex { get; }

        /// <summary>
        /// The donor particle index
        /// </summary>
        public int AcceptorIndex { get; }

        /// <summary>
        /// Creates new state pair from donor and acceptor particle index and a position status
        /// </summary>
        /// <param name="donorIndex"></param>
        /// <param name="acceptorIndex"></param>
        /// <param name="positionStatus"></param>
        public StatePair(int donorIndex, int acceptorIndex, PositionStatus positionStatus) : this()
        {
            DonorIndex = donorIndex;
            AcceptorIndex = acceptorIndex;
            PositionStatus = positionStatus;
        }

        /// <summary>
        /// Get a state pair for an unstable position that is always 0 for the acceptor state
        /// </summary>
        /// <param name="donorIndex"></param>
        /// <returns></returns>
        public static StatePair MakeUnstable(int donorIndex)
        {
            return new StatePair(donorIndex, 0, PositionStatus.Unstable);
        }

        /// <summary>
        /// Creates for the correct position status, i.e. the acceptor state of unstables will be corrected to 0
        /// </summary>
        /// <param name="donorIndex"></param>
        /// <param name="acceptorIndex"></param>
        /// <param name="positionStatus"></param>
        /// <returns></returns>
        public static StatePair CreateForStatus(int donorIndex, int acceptorIndex, PositionStatus positionStatus)
        {
            return (positionStatus == PositionStatus.Unstable) ? MakeUnstable(donorIndex) : new StatePair(donorIndex, acceptorIndex, positionStatus);
        }
    }
}
