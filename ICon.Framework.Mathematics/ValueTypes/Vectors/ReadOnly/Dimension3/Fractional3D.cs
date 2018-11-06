using System.Runtime.Serialization;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Basic fractional vector that carries fractional affine coordinate system information (A,B,C)
    /// </summary>
    [DataContract]
    public readonly struct Fractional3D : IFractional3D<Fractional3D>
    {
        /// <summary>
        ///     The null vector of this type
        /// </summary>
        public static readonly Fractional3D NullVector = new Fractional3D(0, 0, 0);

        /// <inheritdoc />
        public Coordinates<double, double, double> Coordinates { get; }

        /// <inheritdoc />
        [DataMember]
        public double A => Coordinates.A;

        /// <inheritdoc />
        [DataMember]
        public double B => Coordinates.B;

        /// <inheritdoc />
        [DataMember]
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
            Coordinates = new Coordinates<double, double, double>(a, b, c);
        }

        /// <summary>
        ///     Creates new fractional vector from 3D coordinate tuple
        /// </summary>
        /// <param name="coordinates"></param>
        public Fractional3D(Coordinates<double, double, double> coordinates)
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
            return new Fractional3D(A.PeriodicTrim(0.0, 1.0, tolerance), B.PeriodicTrim(0.0, 1.0, tolerance),
                C.PeriodicTrim(0.0, 1.0, tolerance));
        }

        /// <inheritdoc />
        public Fractional3D CreateNew(double a, double b, double c)
        {
            return new Fractional3D(a, b, c);
        }

        /// <inheritdoc />
        public Fractional3D CreateNew(Coordinates<double, double, double> coordinates)
        {
            return new Fractional3D(coordinates);
        }

        /// <summary>
        ///     Vector addition
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator +(Fractional3D lhs, Fractional3D rhs)
        {
            return new Fractional3D(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C);
        }

        /// <summary>
        ///     Multiplication with scalar value
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator *(Fractional3D lhs, double rhs)
        {
            return new Fractional3D(lhs.A * rhs, lhs.B * rhs, lhs.C * rhs);
        }

        /// <summary>
        ///     Vector subtraction
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator -(Fractional3D lhs, Fractional3D rhs)
        {
            return new Fractional3D(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C);
        }

        /// <summary>
        ///     Vector addition
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator +(Fractional3D lhs, IFractional3D rhs)
        {
            return new Fractional3D(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C);
        }

        /// <summary>
        ///     Vector subtraction
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator -(Fractional3D lhs, IFractional3D rhs)
        {
            return new Fractional3D(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C);
        }

        /// <summary>
        ///     Get vector values as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $" Fractional ({A}, {B}, {C})";
        }

        /// <summary>
        ///     Get the vector to the middle between two fractional vectors
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Fractional3D GetMiddle(in Fractional3D lhs, in Fractional3D rhs)
        {
            return (lhs + rhs) * 0.5;
        }
    }
}