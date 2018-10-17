using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    /// Represents a decode version of a property group which is completly describes by particle indices
    /// </summary>
    public readonly struct StatePairGroup
    {
        /// <summary>
        /// Defines the position status of the state pair group
        /// </summary>
        public PositionStatus PositionStatus { get; }

        /// <summary>
        /// The particle index decoded state pairs belonging to the group
        /// </summary>
        public (int Donor, int Acceptor)[] StatePairs { get; }

        /// <summary>
        /// Create new state pair group from a set of donor and acceptor states and the provided position status
        /// </summary>
        /// <param name="statePairs"></param>
        /// <param name="positionStatus"></param>
        public StatePairGroup((int DonorIndex, int AcceptorIndex)[] statePairs, PositionStatus positionStatus) : this()
        {
            StatePairs = statePairs ?? throw new ArgumentNullException(nameof(statePairs));
            PositionStatus = positionStatus;
        }

        /// <summary>
        /// Create new state pair group from a set of donor and acceptor states with undefined position status
        /// </summary>
        /// <param name="statePairs"></param>
        public StatePairGroup((int DonorIndex, int AcceptorIndex)[] statePairs) : this(statePairs, PositionStatus.Undefined)
        {

        }

        /// <summary>
        /// Automatically generates a copy of the state pair group with the position flag set to unstable if any of the acceptor is the void (0) index
        /// or stable otherwise
        /// </summary>
        /// <returns></returns>
        public StatePairGroup AutoChangeStatus()
        {
            if (StatePairs.Any(value => value.Acceptor == 0))
            {
                return GetStatusChanged(PositionStatus.Unstable);
            }
            return GetStatusChanged(PositionStatus.Stable);
        }

        /// <summary>
        /// Returns a new state pair group that contains the same information but carries a new position status
        /// </summary>
        /// <param name="status"></param>
        public StatePairGroup GetStatusChanged(PositionStatus status)
        {
            return new StatePairGroup(StatePairs, status);
        }

        /// <summary>
        /// Creates an empty state pair group
        /// </summary>
        /// <returns></returns>
        public static StatePairGroup CreateEmpty()
        {
            return new StatePairGroup(new (int, int)[0], PositionStatus.Undefined);
        }
    }
}
