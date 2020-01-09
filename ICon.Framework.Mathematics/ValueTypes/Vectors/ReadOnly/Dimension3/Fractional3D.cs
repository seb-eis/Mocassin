using System.Collections.Generic;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Basic fractional vector that carries fractional affine coordinate system information (A,B,C)
    /// </summary>
    public readonly struct Fractional3D : IFractional3D
    {
        /// <summary>
        ///     The readonly <see cref="Coordinates3D"/> backing field
        /// </summary>
        public readonly Coordinates3D Coordinates;

        /// <summary>
        ///     The zero vector (0,0,0) of this type
        /// </summary>
        public static readonly Fractional3D Zero = new Fractional3D(0, 0, 0);

        /// <inheritdoc />
        Coordinates3D IVector3D.Coordinates => Coordinates;

        /// <inheritdoc />
        public double A => Coordinates.A;

        /// <inheritdoc />
        public double B => Coordinates.B;

        /// <inheritdoc />
        public double C => Coordinates.C;

        /// <summary>
        ///     Construct from fractional A,B,C information
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Fractional3D(double a, double b, double c)
            : this()
        {
            Coordinates = new Coordinates3D(a, b, c);
        }

        /// <summary>
        ///     Creates new fractional vector from 3D coordinate tuple
        /// </summary>
        /// <param name="coordinates"></param>
        public Fractional3D(in Coordinates3D coordinates)
            : this()
        {
            Coordinates = coordinates;
        }

        /// <summary>
        ///     Trims the fractional vector into [0.0;1.0) range of the unit cell
        /// </summary>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public Fractional3D TrimToUnitCell(double tolerance)
        {
            return new Fractional3D(
                A.PeriodicTrim(0.0, 1.0, tolerance),
                B.PeriodicTrim(0.0, 1.0, tolerance),
                C.PeriodicTrim(0.0, 1.0, tolerance));
        }

        /// <summary>
        ///     Trims the fractional vector into [0.0;1.0) range of the unit cell
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public Fractional3D TrimToUnitCell(IComparer<double> comparer)
        {
            return new Fractional3D(
                A.PeriodicTrim(0.0, 1.0, comparer),
                B.PeriodicTrim(0.0, 1.0, comparer),
                C.PeriodicTrim(0.0, 1.0, comparer));
        }

        /// <summary>
        ///     Vector addition
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator +(in Fractional3D lhs, in Fractional3D rhs)
        {
            return new Fractional3D(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C);
        }

        /// <summary>
        ///     Multiplication with scalar value
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator *(in Fractional3D lhs, double rhs)
        {
            return new Fractional3D(lhs.A * rhs, lhs.B * rhs, lhs.C * rhs);
        }

        /// <summary>
        ///     Vector subtraction
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator -(in Fractional3D lhs, in Fractional3D rhs)
        {
            return new Fractional3D(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C);
        }

        /// <summary>
        ///     Vector addition
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator +(in Fractional3D lhs, IFractional3D rhs)
        {
            return new Fractional3D(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C);
        }

        /// <summary>
        ///     Vector subtraction
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator -(in Fractional3D lhs, IFractional3D rhs)
        {
            return new Fractional3D(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C);
        }

        /// <summary>
        ///     Get vector values as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Fractional3D ({A}, {B}, {C})";
        }

        /// <summary>
        ///     Get the vector to the middle between two fractional vectors
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Fractional3D CalculateMiddle(in Fractional3D lhs, in Fractional3D rhs)
        {
            return (lhs + rhs) * 0.5;
        }
    }
}