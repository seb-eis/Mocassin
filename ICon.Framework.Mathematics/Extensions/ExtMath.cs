using System;
using System.Collections;
using System.Collections.Generic;

using ICon.Mathematics.Comparers;

namespace ICon.Mathematics.Extensions
{
    /// <summary>
    /// Extends the System.Math implementation with additional functionality and constants in radian
    /// </summary>
    public static class ExtMath
    {
        /// <summary>
        /// Angle (45°) in radian
        /// </summary>
        public const Double Radian45 = 0.25 * Math.PI;

        /// <summary>
        /// Right angle (90°) in radian
        /// </summary>
        public const Double Radian90 = 0.5 * Math.PI;

        /// <summary>
        /// Hexagonal angle (120°) in radian
        /// </summary>
        public const Double Radian120 = (2.0 / 3.0) * Math.PI;

        /// <summary>
        /// Full angle (360°) in radian
        /// </summary>
        public const Double Radian360 = 2.0 * Math.PI;

        /// <summary>
        /// Transforms a degree double avlue to radian double
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Double DegreeToRadian(Double degree)
        {
            return (degree / 180.0) * Math.PI;
        }

        /// <summary>
        /// Transforms a radian double value to degree double
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static Double RadianToDegree(Double radian)
        {
            return (radian / Math.PI) * 180.0;
        }

        /// <summary>
        /// Checks if a value is zero using the provided double comparer (Throws if the tolerance comparer does not support safe zero comparisons)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static Boolean SafeIsZero(Double value, DoubleComparer comparer)
        {
            if (comparer.ZeroSafe == false)
            {
                throw new ArgumentException("Tolerance comparer object cannot be used to safely compare a value to zero", nameof(comparer));
            }
            return comparer.Equals(value, 0.0);
        }

        /// <summary>
        /// Rounds a double value to the largest integer that compares less or equal to value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static Double Floor(Double value, IEqualityComparer<Double> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            Double floor = Math.Floor(value);
            Double ceil = floor + 1.0;
            if (comparer.Equals(value, floor))
            {
                return floor;
            }
            if (comparer.Equals(value, ceil))
            {
                return ceil;
            }
            return floor;
        }

        /// <summary>
        /// Rounds a double value to the largest Int32 that compares less or equal to value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static Int32 Int32Floor(Double value, IEqualityComparer<Double> comparer)
        {
            return (Int32)Floor(value, comparer);
        }

        /// <summary>
        /// Rounds the provided double value to the largest integer that compares less or equal to value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int FloorToInt(double value, IEqualityComparer<double> comparer)
        {
            return (int)Floor(value, comparer);
        }

        /// <summary>
        /// Get the volume of  apshere with the provided radius
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static double GetSphereVolume(double radius)
        {
            return (4.0 / 3.0) * Math.PI * Math.Pow(radius, 3);
        }
    }
}
