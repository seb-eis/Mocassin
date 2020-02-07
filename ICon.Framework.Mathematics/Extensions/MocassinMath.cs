using System;
using System.Collections.Generic;
using Mocassin.Mathematics.Comparer;

namespace Mocassin.Mathematics.Extensions
{
    /// <summary>
    ///     Extends the System.Math implementation with additional functionality and constants in radian
    /// </summary>
    public static class MocassinMath
    {
        /// <summary>
        ///     Angle (45°) in radian
        /// </summary>
        public const double Radian45 = 0.25 * Math.PI;

        /// <summary>
        ///     Right angle (90°) in radian
        /// </summary>
        public const double Radian90 = 0.5 * Math.PI;

        /// <summary>
        ///     Hexagonal angle (120°) in radian
        /// </summary>
        public const double Radian120 = 2.0 / 3.0 * Math.PI;

        /// <summary>
        ///     Full angle (360°) in radian
        /// </summary>
        public const double Radian360 = 2.0 * Math.PI;

        /// <summary>
        ///     Transforms a degree double value to radian double
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static double DegreeToRadian(double degree)
        {
            return degree / 180.0 * Math.PI;
        }

        /// <summary>
        ///     Transforms a radian double value to degree double
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static double RadianToDegree(double radian)
        {
            return radian / Math.PI * 180.0;
        }

        /// <summary>
        ///     Checks if a value is zero using the provided double comparer (Throws if the tolerance comparer does not support
        ///     safe zero comparisons)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool SaveIsZero(double value, NumericComparer comparer)
        {
            if (comparer.IsZeroCompatible == false)
                throw new ArgumentException("Tolerance comparer object cannot be used to safely compare a value to zero", nameof(comparer));
            return comparer.Equals(value, 0.0);
        }

        /// <summary>
        ///     Rounds a double value to the largest integer that compares less or equal to value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static double Floor(this double value, IEqualityComparer<double> comparer)
        {
            var floor = Math.Floor(value);
            var ceil = floor + 1.0;
            return comparer.Equals(value, ceil) ? ceil : floor;
        }

        /// <summary>
        ///     Rounds the provided double value to the largest integer that compares less or equal to value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int FloorToInt(this double value, IEqualityComparer<double> comparer)
        {
            return (int) Floor(value, comparer);
        }

        /// <summary>
        ///     Rounds the provided double value to the largest integer that compares less or equal to value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int FloorToInt(this double value)
        {
            return (int) Math.Floor(value);
        }

        /// <summary>
        ///     Rounds the provided double value to the smallest integer that compares greater or equal to value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CeilToInt(this double value)
        {
            return (int) Math.Ceiling(value);
        }

        /// <summary>
        ///     Rounds the provided double value to an integer
        /// </summary>
        /// <param name="value"></param>
        /// <param name="midpointRounding"></param>
        /// <returns></returns>
        public static int RoundToInt(this double value, MidpointRounding midpointRounding = MidpointRounding.ToEven)
        {
            return (int) Math.Round(value, midpointRounding);
        }

        /// <summary>
        ///     Get the volume of a sphere with the provided radius
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static double GetSphereVolume(double radius)
        {
            return 4.0 / 3.0 * Math.PI * Math.Pow(radius, 3);
        }
    }
}