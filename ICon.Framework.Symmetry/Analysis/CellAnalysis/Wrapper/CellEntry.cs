using System;
using System.Collections.Generic;
using System.Text;
using ICon.Mathematics.ValueTypes;
using ICon.Framework.Collections;

namespace ICon.Symmetry.Analysis
{
    /// <summary>
    /// Basic cell entry wrapper that contains an entry information and a fractional vector describing its absolute position
    /// </summary>
    public readonly struct CellEntry<T1>
    {
        /// <summary>
        /// The absolute position of the cell entry
        /// </summary>
        public Fractional3D AbsoluteVector { get; }

        /// <summary>
        /// The entry affiliated with the position
        /// </summary>
        public T1 Entry { get; }

        /// <summary>
        /// Create new unit cell entry
        /// </summary>
        /// <param name="absoluteVector"></param>
        /// <param name="entry"></param>
        public CellEntry(Fractional3D absoluteVector, T1 entry) : this()
        {
            AbsoluteVector = absoluteVector;
            Entry = entry;
        }

        /// <summary>
        /// Get a shifted version of the unit cell entry
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public CellEntry<T1> GetShifted(in Fractional3D offset)
        {
            return new CellEntry<T1>(AbsoluteVector + offset, Entry);
        }

        /// <summary>
        /// Creates a combined vector and entry comparer from two seperated comparer interfaces
        /// </summary>
        /// <param name="vectorComparer"></param>
        /// <param name="entryComparer"></param>
        /// <returns></returns>
        public static IComparer<CellEntry<T1>> MakeComparer(IComparer<Fractional3D> vectorComparer, IComparer<T1> entryComparer)
        {
            if (vectorComparer == null)
            {
                throw new ArgumentNullException(nameof(vectorComparer));
            }
            if (entryComparer == null)
            {
                throw new ArgumentNullException(nameof(entryComparer));
            }

            return Comparer<CellEntry<T1>>.Create((lhs, rhs) =>
            {
                int vectorCompare = vectorComparer.Compare(lhs.AbsoluteVector, rhs.AbsoluteVector);
                if (vectorCompare == 0)
                {
                    return entryComparer.Compare(lhs.Entry, rhs.Entry);
                }
                return vectorCompare;
            });
        }
    }
}
