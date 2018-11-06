using System;
using System.Collections.Generic;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Basic cartesian vector that carries not additional information other than X,Y,Z coordinates
    /// </summary>
    public readonly struct Cartesian3D : ICartesian3D<Cartesian3D>
    {
        /// <summary>
        ///     The 3D coordinate tuple
        /// </summary>
        public Coordinates<double, double, double> Coordinates { get; }

        /// <summary>
        ///     Get the X coordinate value
        /// </summary>
        public double X => Coordinates.A;

        /// <summary>
        ///     Get the Y coordinate value
        /// </summary>
        public double Y => Coordinates.B;

        /// <summary>
        ///     Get the Z coordinate value
        /// </summary>
        public double Z => Coordinates.C;

        /// <summary>
        ///     Construct from X,Y,Z coordinate values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Cartesian3D(double x, double y, double z)
            : this()
        {
            Coordinates = new Coordinates<double, double, double>(x, y, z);
        }

        /// <summary>
        ///     Creates new cartesian vector from 3D coordinate tuple
        /// </summary>
        /// <param name="coordinates"></param>
        public Cartesian3D(Coordinates<double, double, double> coordinates)
            : this()
        {
            Coordinates = coordinates;
        }

        /// <summary>
        ///     Factory function to create a new cartesian vector
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public Cartesian3D CreateNew(double a, double b, double c)
        {
            return new Cartesian3D(a, b, c);
        }

        /// <summary>
        ///     Factory function to create a new cartesian vector
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public Cartesian3D CreateNew(Coordinates<double, double, double> coordinates)
        {
            return new Cartesian3D(coordinates);
        }

        /// <summary>
        ///     Checks if three cartesian vectors form a linear indendent system
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="vecB"></param>
        /// <param name="vecC"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool IsLinearIndependetFrom(in Cartesian3D vecA, in Cartesian3D vecB, IEqualityComparer<double> comparer)
        {
            return Coordinates.IsLinearIndependentFrom(vecA.Coordinates, vecB.Coordinates, comparer);
        }

        /// <summary>
        ///     Checks if the cartesian vector is linear independent from another
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool IsLinearIndependetFrom(in Cartesian3D vecA, IEqualityComparer<double> comparer)
        {
            return Coordinates.IsLinearIndependentFrom(vecA.Coordinates, comparer);
        }

        /// <summary>
        ///     Vector addition
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Cartesian3D operator +(Cartesian3D lhs, Cartesian3D rhs)
        {
            return new Cartesian3D(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        /// <summary>
        ///     Vector substraction
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Cartesian3D operator -(Cartesian3D lhs, Cartesian3D rhs)
        {
            return new Cartesian3D(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        /// <summary>
        ///     Vector addition
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Cartesian3D operator +(Cartesian3D lhs, ICartesian3D rhs)
        {
            return new Cartesian3D(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        /// <summary>
        ///     Vector substraction
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Cartesian3D operator -(Cartesian3D lhs, ICartesian3D rhs)
        {
            return new Cartesian3D(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        /// <summary>
        ///     Scalar product of two cartesian vectors
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static double operator *(Cartesian3D lhs, Cartesian3D rhs)
        {
            return lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;
        }

        /// <summary>
        ///     Multiplication with scalar value
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Cartesian3D operator *(Cartesian3D lhs, double scalar)
        {
            return new Cartesian3D(lhs.X * scalar, lhs.Y * scalar, lhs.Z * scalar);
        }

        /// <summary>
        ///     Division by scalar value
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Cartesian3D operator /(Cartesian3D lhs, double scalar)
        {
            return lhs * (1.0 / scalar);
        }

        /// <summary>
        ///     Get the length of the vector
        /// </summary>
        /// <returns></returns>
        public double GetLength()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }
    }
}