using System;
using System.Linq;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Represents a decode version of a property group which is completely describes by particle indices
    /// </summary>
    public readonly struct StatePairGroup
    {
        /// <summary>
        ///     Defines the position status of the state pair group
        /// </summary>
        public PositionStability PositionStability { get; }

        /// <summary>
        ///     The particle index decoded state pairs belonging to the group
        /// </summary>
        public (int Donor, int Acceptor)[] StatePairs { get; }

        /// <summary>
        ///     Create new state pair group from a set of donor and acceptor states and the provided position status
        /// </summary>
        /// <param name="statePairs"></param>
        /// <param name="positionStability"></param>
        public StatePairGroup((int DonorIndex, int AcceptorIndex)[] statePairs, PositionStability positionStability)
            : this()
        {
            StatePairs = statePairs ?? throw new ArgumentNullException(nameof(statePairs));
            PositionStability = positionStability;
        }

        /// <summary>
        ///     Create new state pair group from a set of donor and acceptor states with stable position status
        /// </summary>
        /// <param name="statePairs"></param>
        public StatePairGroup((int DonorIndex, int AcceptorIndex)[] statePairs)
            : this(statePairs, PositionStability.Stable)
        {
        }

        /// <summary>
        ///     Automatically generates a copy of the state pair group with the position flag set to unstable if any of the
        ///     acceptor is the void (0) index
        ///     or stable otherwise
        /// </summary>
        /// <returns></returns>
        public StatePairGroup AutoChangeStatus()
        {
            return GetStatusChanged(StatePairs.Any(value => value.Acceptor == 0) ? PositionStability.Unstable : PositionStability.Stable);
        }

        /// <summary>
        ///     Returns a new state pair group that contains the same information but carries a new position status
        /// </summary>
        /// <param name="stability"></param>
        public StatePairGroup GetStatusChanged(PositionStability stability) => new StatePairGroup(StatePairs, stability);

        /// <summary>
        ///     Creates an empty state pair group
        /// </summary>
        /// <returns></returns>
        public static StatePairGroup CreateEmpty() => new StatePairGroup(new (int, int)[0], PositionStability.Stable);
    }
}