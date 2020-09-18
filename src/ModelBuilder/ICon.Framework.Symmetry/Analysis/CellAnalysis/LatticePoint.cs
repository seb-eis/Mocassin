using System;
using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Generic lattice point wrapper that contains an entry information and a fractional vector describing its absolute
    ///     position
    /// </summary>
    public readonly struct LatticePoint<TContent>
    {
        /// <summary>
        ///     The absolute position of the cell entry
        /// </summary>
        public Fractional3D Fractional { get; }

        /// <summary>
        ///     The content affiliated with the position
        /// </summary>
        public TContent Content { get; }

        /// <summary>
        ///     Create new unit cell entry
        /// </summary>
        /// <param name="fractional"></param>
        /// <param name="content"></param>
        public LatticePoint(in Fractional3D fractional, TContent content)
            : this()
        {
            Fractional = fractional;
            Content = content;
        }

        /// <summary>
        ///     Get a shifted version of the unit cell entry
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public LatticePoint<TContent> GetShifted(in Fractional3D offset) => new LatticePoint<TContent>(Fractional + offset, Content);

        /// <summary>
        ///     Creates a combined vector and entry comparer from two separated comparer interfaces
        /// </summary>
        /// <param name="vectorComparer"></param>
        /// <param name="entryComparer"></param>
        /// <returns></returns>
        public static IComparer<LatticePoint<TContent>> MakeComparer(IComparer<Fractional3D> vectorComparer, IComparer<TContent> entryComparer)
        {
            if (vectorComparer == null)
                throw new ArgumentNullException(nameof(vectorComparer));

            if (entryComparer == null)
                throw new ArgumentNullException(nameof(entryComparer));

            return Comparer<LatticePoint<TContent>>.Create((lhs, rhs) =>
            {
                var vectorCompare = vectorComparer.Compare(lhs.Fractional, rhs.Fractional);
                return vectorCompare == 0
                    ? entryComparer.Compare(lhs.Content, rhs.Content)
                    : vectorCompare;
            });
        }
    }
}