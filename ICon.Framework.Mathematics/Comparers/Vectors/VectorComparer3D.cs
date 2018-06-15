using System;
using System.Collections.Generic;

using ICon.Mathematics.ValueTypes;

namespace ICon.Mathematics.Comparers
{
    /// <summary>
    /// Generic comparator object for structs that implement IVector3D, automatically provides generic comparisons for any IVector3D types
    /// </summary>
    public class VectorComparer3D<T1> : Comparer<T1>, IEqualityComparer<T1> where T1 : struct, IVector3D
    {
        /// <summary>
        /// The internal double value comparer for the vector coordinate values
        /// </summary>
        public IComparer<Double> ValueComparer { get; }

        /// <summary>
        /// Creates new vector comparer using the provided comparer interface
        /// </summary>
        /// <param name="valueComparer"></param>
        public VectorComparer3D(IComparer<Double> valueComparer)
        {
            ValueComparer = valueComparer;
        }

        /// <summary>
        /// Test for equality with the internal almost equal comparator
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Boolean Equals(T1 x, T1 y)
        {
            return Compare(x, y) == 0;
        }

        /// <summary>
        /// Get the hash code ob the double vector object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Int32 GetHashCode(T1 obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>
        /// Compares the two vectors
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override Int32 Compare(T1 x, T1 y)
        {
            Int32 compareA = ValueComparer.Compare(x.Coordinates.A, y.Coordinates.A);
            if (compareA == 0)
            {
                Int32 compareB = ValueComparer.Compare(x.Coordinates.B, y.Coordinates.B);
                if (compareB == 0)
                {
                    return ValueComparer.Compare(x.Coordinates.C, y.Coordinates.C);
                }
                return compareB;
            }
            return compareA;
        }

        /// <summary>
        /// Generic compare for two vectors that implement the IVector3D vector of the provided vector type
        /// </summary>
        /// <typeparam name="TVector1"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Int32 Compare<TVector1, TVector2>(TVector1 x, TVector2 y) where TVector1 : struct, IVector3D where TVector2 : struct, IVector3D
        {
            Int32 compareA = ValueComparer.Compare(x.Coordinates.A, y.Coordinates.A);
            if (compareA == 0)
            {
                Int32 compareB = ValueComparer.Compare(x.Coordinates.B, y.Coordinates.B);
                if (compareB == 0)
                {
                    return ValueComparer.Compare(x.Coordinates.C, y.Coordinates.C);
                }
                return compareB;
            }
            return compareA;
        }

        /// <summary>
        /// Creates a new vector 3D comparer for another type with the same comparisons tolerance as the current
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <returns></returns>
        public VectorComparer3D<TVector> MakeCompatibleComparer<TVector>() where TVector : struct, IVector3D
        {
            return new VectorComparer3D<TVector>(ValueComparer);
        }
    }

    /// <summary>
    /// Generic comparator object for structs that implement Vector3D, comapres two different vectors
    /// </summary>
    public class VectorComparer3D<T1, T2> : VectorComparer3D<T1>
        where T1 : struct, IVector3D
        where T2 : struct, IVector3D
    {
        public VectorComparer3D(DoubleComparer comparer) : base(comparer)
        {

        }

        /// <summary>
        /// Test for equality with the internal almost equal comparator
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Boolean Equals(T1 x, T2 y)
        {
            return Compare(x, y) == 0;
        }

        /// <summary>
        /// Compares the two vectors
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Int32 Compare(T1 x, T2 y)
        {
            Int32 compareA = ValueComparer.Compare(x.Coordinates.A, y.Coordinates.A);
            if (compareA == 0)
            {
                Int32 compareB = ValueComparer.Compare(x.Coordinates.B, y.Coordinates.B);
                if (compareB == 0)
                {
                    return ValueComparer.Compare(x.Coordinates.C, y.Coordinates.C);
                }
                return compareB;
            }
            return compareA;
        }
    }
}
