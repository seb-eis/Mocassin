using System;

namespace ICon.Model.Particles
{
    /// <summary>
    ///     Pair code struct that defines a pair interaction between two particles. The first index is always defined to be
    ///     lesser or equal to the second
    /// </summary>
    public readonly struct PairCode : IEquatable<PairCode>, IComparable<PairCode>
    {
        /// <summary>
        ///     The first particle index
        /// </summary>
        public int Index0 { get; }

        /// <summary>
        ///     The second particle index
        /// </summary>
        public int Index1 { get; }

        /// <summary>
        ///     Create new pair code from two particle indices
        /// </summary>
        /// <param name="index0"></param>
        /// <param name="index1"></param>
        public PairCode(int index0, int index1)
            : this()
        {
            Index0 = index0 <= index1 ? index0 : index1;
            Index1 = Index0 == index1 ? index0 : index1;
        }

        /// <inheritdoc />
        public bool Equals(PairCode other)
        {
            return other.Index0 == Index0 && other.Index1 == Index1;
        }

        /// <summary>
        ///     Compares to other pair code
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PairCode other)
        {
            var first = Index0.CompareTo(other.Index0);
            return first != 0 
                ? Index1.CompareTo(other.Index1) 
                : first;
        }
    }
}