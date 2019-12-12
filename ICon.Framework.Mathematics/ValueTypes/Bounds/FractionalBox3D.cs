using System;
using System.Collections.Generic;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Describes a rectangular box in the fractional coordinates that supports <see cref="Fractional3D"/> bounds checking without converting to a cartesian context
    /// </summary>
    public readonly struct FractionalBox3D
    {
        /// <summary>
        ///     Get the <see cref="Fractional3D"/> that describes the start point
        /// </summary>
        public Fractional3D Start { get; }

        /// <summary>
        ///     Get the <see cref="Fractional3D"/> that describes the A,B,C sizes
        /// </summary>
        public Fractional3D Size { get; }

        /// <summary>
        ///     Get the <see cref="Fractional3D"/> that describes the end point
        /// </summary>
        public Fractional3D End { get; }

        /// <summary>
        ///     Creates a new <see cref="FractionalBox3D"/> with start point and size information
        /// </summary>
        /// <param name="start"></param>
        /// <param name="size"></param>
        public FractionalBox3D(in Fractional3D start, in Fractional3D size)
        {
            Size = size;
            (Start, End) = CalculateStartAndEndPoint(start, size);
        }

        /// <summary>
        ///     Performs a within bounds check for the provided a,b,c coordinates using direct double comparison (Strictly within bounds)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool IsWithinBounds(double a, double b, double c)
        {
            return a >= Start.A && a <= End.A &&
                   b >= Start.B && b <= End.B &&
                   c >= Start.C && c <= End.C;
        }

        /// <summary>
        ///     Performs a within bounds check for the provided <see cref="Fractional3D"/> using direct double comparison (Strictly within bounds)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsWithinBounds(in Fractional3D point)
        {
            return IsWithinBounds(point.A, point.B, point.C);
        }

        /// <summary>
        ///     Performs a within bounds check for the provided a,b,c coordinates using a double <see cref="IComparer{T}" />
        /// </summary>
        /// <param name="c"></param>
        /// <param name="comparer"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool IsWithinBounds(double a, double b, double c, IComparer<double> comparer)
        {
            return comparer.Compare(a, Start.A) >= 0 && comparer.Compare(a, End.A) <= 0 &&
                   comparer.Compare(b, Start.B) >= 0 && comparer.Compare(b, End.B) <= 0 &&
                   comparer.Compare(c, Start.C) >= 0 && comparer.Compare(c, End.C) <= 0;
        }

        /// <summary>
        ///     Performs a within bounds check for the provided <see cref="Fractional3D"/> using a double <see cref="IComparer{T}"/>
        /// </summary>
        /// <param name="point"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool IsWithinBounds(in Fractional3D point, IComparer<double> comparer)
        {
            return IsWithinBounds(point.A, point.B, point.C, comparer);
        }

        /// <summary>
        ///     Performs an almost within bounds check for the provided <see cref="Fractional3D"/> using a range tolerance
        /// </summary>
        /// <param name="c"></param>
        /// <param name="tolerance"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public bool IsWithinBounds(double a,double b,double c, double tolerance)
        {
            tolerance = Math.Abs(tolerance);
            return a >= Start.A + tolerance && a <= End.A - tolerance &&
                   b >= Start.B + tolerance && b <= End.B - tolerance &&
                   c >= Start.C + tolerance && c <= End.C - tolerance;
        }

        /// <summary>
        ///     Performs an almost within bounds check for the provided <see cref="Fractional3D"/> using a range tolerance
        /// </summary>
        /// <param name="point"></param>
        /// <param name="tolerance"></param>
        public bool IsWithinBounds(in Fractional3D point, double tolerance)
        {
            return IsWithinBounds(point.A, point.B, point.C, tolerance);
        }

        /// <summary>
        ///     Calculates the <see cref="Fractional3D"/> start and end points of the <see cref="FractionalBox3D"/> from an arbitrary start point and a size information
        /// </summary>
        /// <param name="start"></param>
        /// <param name="size"></param>
        /// <remarks> The resulting end point is component wise greater or equal to the resulting start point components </remarks>
        /// <returns></returns>
        public static (Fractional3D Start, Fractional3D End) CalculateStartAndEndPoint(in Fractional3D start, in Fractional3D size)
        {
            var (aMin, aMax) = size.A < 0 ? (start.A + size.A, start.A) : (start.A, start.A + size.A);
            var (bMin, bMax) = size.B < 0 ? (start.B + size.B, start.B) : (start.B, start.B + size.B);
            var (cMin, cMax) = size.C < 0 ? (start.C + size.C, start.C) : (start.C, start.C + size.C);
            return (new Fractional3D(aMin, bMin, cMin), new Fractional3D(aMax, bMax, cMax));
        }
    }
}