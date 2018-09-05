using System;
using System.Runtime.Serialization;

using ICon.Mathematics.Extensions;


namespace ICon.Mathematics.ValueTypes
{
    /// <summary>
    /// Basic fractional vector that carries fractional affine coordinate system information (A,B,C)
    /// </summary>
    [DataContract]
    public readonly struct Fractional3D : IFractional3D<Fractional3D>
    {
        /// <summary>
        /// The null vector of this type
        /// </summary>
        public static readonly Fractional3D NullVector = new Fractional3D(0,0,0);

        /// <summary>
        /// The 3D coordinate tuple
        /// </summary>
        public Coordinates<Double, Double, Double> Coordinates { get; }

        /// <summary>
        /// Get the X coordinate value
        /// </summary>
        [DataMember]
        public Double A => Coordinates.A;

        /// <summary>
        /// Get the Y coordinate value
        /// </summary>
        [DataMember]
        public Double B => Coordinates.B;

        /// <summary>
        /// Get the Z coordinate value
        /// </summary>
        [DataMember]
        public Double C => Coordinates.C;

        /// <summary>
        /// Construct from fractional A,B,C information
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Fractional3D(Double a, Double b, Double c) : this()
        {
            Coordinates = new Coordinates<Double, Double, Double>(a, b, c);
        }

        /// <summary>
        /// Creates new fractional vector from 3D coordinate tuple
        /// </summary>
        /// <param name="coordinates"></param>
        public Fractional3D(Coordinates<Double, Double, Double> coordinates) : this()
        {
            Coordinates = coordinates;
        }

        /// <summary>
        /// Trims the fractional vector into [0.0;1.0) range of the unit cell
        /// </summary>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public Fractional3D TrimToUnitCell(Double tolerance)
        {
            return new Fractional3D(A.PeriodicTrim(0.0, 1.0, tolerance), B.PeriodicTrim(0.0, 1.0, tolerance), C.PeriodicTrim(0.0, 1.0, tolerance));
        }

        /// <summary>
        /// CFactory function to create new basic fractional vector
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public Fractional3D CreateNew(Double a, Double b, Double c)
        {
            return new Fractional3D(a, b, c);
        }

        /// <summary>
        /// Factory function to create new basic fractional vector
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public Fractional3D CreateNew(Coordinates<Double, Double, Double> coordinates)
        {
            return new Fractional3D(coordinates);
        }

        /// <summary>
        /// Vector addition
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator+(Fractional3D lhs, Fractional3D rhs)
        {
            return new Fractional3D(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C);
        }

        /// <summary>
        /// Multiplication with scalar value
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator *(Fractional3D lhs, double rhs)
        {
            return new Fractional3D(lhs.A * rhs, lhs.B * rhs, lhs.C * rhs);
        }

        /// <summary>
        /// Vector substraction
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator -(Fractional3D lhs, Fractional3D rhs)
        {
            return new Fractional3D(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C);
        }

        /// <summary>
        /// Vector addition
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator +(Fractional3D lhs, IFractional3D rhs)
        {
            return new Fractional3D(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C);
        }

        /// <summary>
        /// Vector substraction
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Fractional3D operator -(Fractional3D lhs, IFractional3D rhs)
        {
            return new Fractional3D(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C);
        }

        /// <summary>
        /// Get vectior values as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $" Fractional ({A.ToString()}, {B.ToString()}, {C.ToString()})";
        }

        /// <summary>
        /// Get the vector to the middle between two fractional vectors
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Fractional3D GetMiddle(in Fractional3D lhs, in Fractional3D rhs)
        {
            return (lhs + rhs) * 0.5;
        }
    }
}
