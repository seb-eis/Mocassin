using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;

namespace ICon.Mathematics.Extensions
{
    /// <summary>
    ///  Extension calss for the coordinate tuple that supplies generic comparsion methods and other convenience functions
    /// </summary>
    public static class CoordinateExtensions
    {
        /// <summary>
        /// Compares a three entry coordinate tuple where all entries support the IComparable implementation
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static int CompareCoordinates<T1, T2, T3>(this Coordinates<T1, T2, T3> lhs, in Coordinates<T1, T2, T3> rhs)
            where T1 : struct, IComparable<T1>
            where T2 : struct, IComparable<T2>
            where T3 : struct, IComparable<T3>
        {
            int compA = lhs.A.CompareTo(rhs.A);
            if (compA == 0)
            {
                int compB = lhs.B.CompareTo(rhs.B);
                if (compB == 0)
                {
                    return lhs.C.CompareTo(rhs.C);
                }
                return compB;
            }
            return compA;
        }

        /// <summary>
        /// Compares a three entry coordinate tuple for equality where all entries support IEquatable
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool CoordinateEqual<T1, T2, T3>(this Coordinates<T1, T2, T3> lhs, in Coordinates<T1, T2, T3> rhs)
            where T1 : struct, IEquatable<T1>
            where T2 : struct, IEquatable<T2>
            where T3 : struct, IEquatable<T3>
        {
            return lhs.A.Equals(rhs.A) && lhs.B.Equals(rhs.B) && lhs.C.Equals(rhs.B);
        }

            /// <summary>
            /// Compares a four entry coordinate tuple where all entries support the IComparable implementation
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <typeparam name="T3"></typeparam>
            /// <typeparam name="T4"></typeparam>
            /// <param name="lhs"></param>
            /// <param name="rhs"></param>
            /// <returns></returns>
            public static int CompareCoordinates<T1, T2, T3, T4>(this Coordinates<T1, T2, T3, T4> lhs, in Coordinates<T1, T2, T3, T4> rhs)
             where T1 : struct, IComparable<T1>
             where T2 : struct, IComparable<T2>
             where T3 : struct, IComparable<T3>
             where T4 : struct, IComparable<T4>
        {
            int compA = lhs.A.CompareTo(rhs.A);
            if (compA == 0)
            {
                int compB = lhs.B.CompareTo(rhs.B);
                if (compB == 0)
                {
                    int compC = lhs.C.CompareTo(rhs.C);
                    if (compC == 0)
                    {
                        return lhs.D.CompareTo(rhs.D);
                    }
                    return compC;
                }
                return compB;
            }
            return compA;
        }

        /// <summary>
        /// Compares a four entry coordinate tuple for equality where all entries support IEquatable
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool CoordinatesEqual<T1, T2, T3, T4>(this Coordinates<T1, T2, T3, T4> lhs, in Coordinates<T1, T2, T3, T4> rhs)
            where T1 : struct, IEquatable<T1>
            where T2 : struct, IEquatable<T2>
            where T3 : struct, IEquatable<T3>
            where T4 : struct, IEquatable<T4>
        {
            return lhs.A.Equals(rhs.A) && lhs.B.Equals(rhs.B) && lhs.C.Equals(rhs.B) && lhs.D.Equals(rhs.D);
        }
    }
}
