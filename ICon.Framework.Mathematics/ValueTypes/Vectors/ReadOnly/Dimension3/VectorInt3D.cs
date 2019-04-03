using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Mocassin.Mathematics.ValueTypes;

namespace Moccasin.Mathematics.ValueTypes
{
    /// <summary>
    /// Basic cartesian vector that carries not additional information other than X,Y,Z coordinates
    /// </summary>
    public readonly struct VectorInt3D
    {
        /// <summary>
        /// The 3D coordinate tuple
        /// </summary>
        public Coordinates<int, int, int> Coordinates { get; }

        /// <summary>
        /// Get the X coordinate value
        /// </summary>
        public int A => Coordinates.A;

        /// <summary>
        /// Get the Y coordinate value
        /// </summary>
        public int B => Coordinates.B;

        /// <summary>
        /// Get the Z coordinate value
        /// </summary>
        public int C => Coordinates.C;

        /// <summary>
        /// Construct from A,B,C coordinate values
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public VectorInt3D(int a, int b, int c) : this()
        {
            Coordinates = new Coordinates<int, int, int>(a, b, c);
        }

        /// <summary>
        /// Construct from DataIntVector3D
        /// </summary>
        /// <param name="vector"></param>
        public VectorInt3D(DataIntVector3D vector) : this()
        {
            Coordinates = new Coordinates<int, int, int>(vector.A, vector.B, vector.C);
        }

        /// <summary>
        /// Creates new cartesian vector from 3D coordinate tuple
        /// </summary>
        /// <param name="coordinates"></param>
        public VectorInt3D(Coordinates<int, int, int> coordinates) : this()
        {
            Coordinates = coordinates;
        }

        /// <summary>
        /// Factory function to create a new cartesian vector
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public VectorInt3D CreateNew(int a, int b, int c)
        {
            return new VectorInt3D(a, b, c);
        }

        /// <summary>
        /// Factory function to create a new cartesian vector
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public VectorInt3D CreateNew(Coordinates<int, int, int> coordinates)
        {
            return new VectorInt3D(coordinates);
        }

        /// <summary>
        /// Vector addition
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static VectorInt3D operator +(VectorInt3D lhs, VectorInt3D rhs)
        {
            return new VectorInt3D(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C);
        }

        /// <summary>
        /// Vector substraction
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static VectorInt3D operator -(VectorInt3D lhs, VectorInt3D rhs)
        {
            return new VectorInt3D(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C);
        }

        /// <summary>
        /// Scalar product of two cartesian vectors
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static double operator *(VectorInt3D lhs, VectorInt3D rhs)
        {
            return lhs.A * rhs.A + lhs.B * rhs.B + lhs.C * rhs.C;
        }

        /// <summary>
        /// Multiplication with scalar value
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static VectorInt3D operator *(VectorInt3D lhs, int scalar)
        {
            return new VectorInt3D(lhs.A * scalar, lhs.B * scalar, lhs.C * scalar);
        }

        /// <summary>
        /// Division by scalar value
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static VectorInt3D operator /(VectorInt3D lhs, int scalar)
        {
            return new VectorInt3D(lhs.A / scalar, lhs.B / scalar, lhs.C / scalar);
        }

        /// <summary>
        /// Division by vector
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static VectorInt3D operator /(VectorInt3D lhs, VectorInt3D rhs)
        {
            return new VectorInt3D(lhs.A / rhs.A, lhs.B / rhs.B, lhs.C / rhs.C);
        }

        /// <summary>
        /// Remainder operation of every component by scalar value
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static VectorInt3D operator %(VectorInt3D lhs, int scalar)
        {
            return new VectorInt3D(lhs.A % scalar, lhs.B % scalar, lhs.C % scalar);
        }

        /// <summary>
        /// Remainder operation of every component by scalar value
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static VectorInt3D operator %(VectorInt3D lhs, VectorInt3D rhs)
        {
            return new VectorInt3D(lhs.A % rhs.A, lhs.B % rhs.B, lhs.C % rhs.C);
        }

        /// <summary>
        /// Get the length of the vector
        /// </summary>
        /// <returns></returns>
        public double GetLength()
        {
            return Math.Sqrt(A * A + B * B + C * C);
        }

        /// <summary>
        /// Get the volume of a cuboid generated by this vector
        /// </summary>
        /// <returns></returns>
        public int GetCoordinateProduct()
        {
            return A * B * C;
        }

        /// <summary>
        /// Compares the components of two vectors for equality
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool Equals(VectorInt3D vector)
        {
            return A == vector.A && B == vector.B && C == vector.C;
        }
    }
}
