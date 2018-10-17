using System;
using System.Collections.Generic;
using System.Text;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    /// Mapping for a metropolis transition that describes an exchange between two unit cell positions
    /// </summary>
    public readonly struct MetropolisMapping : IComparable<MetropolisMapping>, IEquatable<MetropolisMapping>
    {
        /// <summary>
        /// The parent metropolis transition this mapping is valid for
        /// </summary>
        public IMetropolisTransition Transition { get; }

        /// <summary>
        /// Index of the first position
        /// </summary>
        public int PositionIndex0 { get; }

        /// <summary>
        /// Index of the second position
        /// </summary>
        public int PositionIndex1 { get; }

        /// <summary>
        /// Create new metropolis mapping from two exchanging position indices and the parent transition
        /// </summary>
        /// <param name="firstPositionIndex"></param>
        /// <param name="secondPositionIndex"></param>
        /// <param name="transition"></param>
        public MetropolisMapping(IMetropolisTransition transition, int firstPositionIndex, int secondPositionIndex) : this()
        {
            Transition = transition;
            PositionIndex0 = firstPositionIndex;
            PositionIndex1 = secondPositionIndex;
        }

        /// <summary>
        /// Compares two metropolis mappings by their position idnices
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(MetropolisMapping other)
        {
            var firstComp = PositionIndex0.CompareTo(other.PositionIndex0);
            if (firstComp == 0)
            {
                return PositionIndex1.CompareTo(other.PositionIndex1);
            }
            return firstComp;
        }

        /// <summary>
        /// Checks two metropolis mappings for equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MetropolisMapping other)
        {
            return PositionIndex0 == other.PositionIndex0 && PositionIndex1 == other.PositionIndex1;
        }
    }
}
