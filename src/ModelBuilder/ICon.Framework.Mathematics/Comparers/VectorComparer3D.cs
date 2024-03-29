﻿using System;
using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.Comparer
{
    /// <summary>
    ///     Generic comparator object for structs that implement IVector3D, automatically provides generic comparisons for any
    ///     IVector3D types
    /// </summary>
    public class VectorComparer3D<T> : Comparer<T>, IEqualityComparer<T>, IComparer<IVector3D> where T : IVector3D
    {
        /// <summary>
        ///     The internal double value comparer for the vector coordinate values
        /// </summary>
        public IComparer<double> ValueComparer { get; }

        /// <summary>
        ///     Creates new vector comparer using the provided comparer interface
        /// </summary>
        /// <param name="valueComparer"></param>
        public VectorComparer3D(IComparer<double> valueComparer)
        {
            ValueComparer = valueComparer;
        }

        /// <summary>
        ///     Implementation of generic 3D vector interface comparison
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        int IComparer<IVector3D>.Compare(IVector3D x, IVector3D y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (y == null)
                throw new ArgumentNullException(nameof(y));

            var compareA = ValueComparer.Compare(x.Coordinates.A, y.Coordinates.A);
            if (compareA != 0)
                return compareA;

            var compareB = ValueComparer.Compare(x.Coordinates.B, y.Coordinates.B);
            return compareB == 0
                ? ValueComparer.Compare(x.Coordinates.C, y.Coordinates.C)
                : compareB;
        }

        /// <inheritdoc />
        public bool Equals(T x, T y) => Compare(x, y) == 0;

        /// <inheritdoc />
        public int GetHashCode(T obj) => obj.GetHashCode();

        /// <summary>
        ///     Compares the two vectors
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(T x, T y)
        {
            if (x == null) throw new ArgumentNullException(nameof(x));
            if (y == null) throw new ArgumentNullException(nameof(y));
            var compareA = ValueComparer.Compare(x.Coordinates.A, y.Coordinates.A);
            if (compareA != 0)
                return compareA;

            var compareB = ValueComparer.Compare(x.Coordinates.B, y.Coordinates.B);
            return compareB == 0
                ? ValueComparer.Compare(x.Coordinates.C, y.Coordinates.C)
                : compareB;
        }

        /// <summary>
        ///     Generic compare for two vectors that implement the IVector3D vector of the provided vector type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare<T1, T2>(T1 x, T2 y) where T1 : IVector3D where T2 : IVector3D
        {
            var compareA = ValueComparer.Compare(x.Coordinates.A, y.Coordinates.A);
            if (compareA != 0)
                return compareA;

            var compareB = ValueComparer.Compare(x.Coordinates.B, y.Coordinates.B);
            return compareB == 0
                ? ValueComparer.Compare(x.Coordinates.C, y.Coordinates.C)
                : compareB;
        }

        /// <summary>
        ///     Creates a new vector 3D comparer for another type with the same comparisons tolerance as the current
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <returns></returns>
        public VectorComparer3D<TVector> ToCompatibleComparer<TVector>() where TVector : IVector3D => new VectorComparer3D<TVector>(ValueComparer);
    }

    /// <inheritdoc />
    /// <remarks> Version for two different vector types </remarks>
    public class VectorComparer3D<T1, T2> : VectorComparer3D<T1>
        where T1 : struct, IVector3D
        where T2 : struct, IVector3D
    {
        /// <inheritdoc />
        public VectorComparer3D(NumericComparer comparer)
            : base(comparer)
        {
        }

        /// <summary>
        ///     Test for equality with the internal almost equal comparator
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(T1 x, T2 y) => Compare(x, y) == 0;

        /// <summary>
        ///     Compares the two vectors
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(T1 x, T2 y)
        {
            var compareA = ValueComparer.Compare(x.Coordinates.A, y.Coordinates.A);
            if (compareA != 0)
                return compareA;

            var compareB = ValueComparer.Compare(x.Coordinates.B, y.Coordinates.B);
            return compareB == 0
                ? ValueComparer.Compare(x.Coordinates.C, y.Coordinates.C)
                : compareB;
        }
    }
}