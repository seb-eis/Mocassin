﻿using System;
using System.Collections.Generic;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Basic cartesian vector that carries not additional information other than X,Y,Z coordinates
    /// </summary>
    public readonly struct Cartesian3D : ICartesian3D
    {
        /// <summary>
        ///     The readonly <see cref="Coordinates3D" /> backing field
        /// </summary>
        public readonly Coordinates3D Coordinates;

        /// <summary>
        ///     The zero vector (0,0,0) of this type
        /// </summary>
        public static readonly Cartesian3D Zero = new Cartesian3D(0, 0, 0);

        /// <inheritdoc />
        Coordinates3D IVector3D.Coordinates => Coordinates;

        /// <inheritdoc />
        public double X => Coordinates.A;

        /// <inheritdoc />
        public double Y => Coordinates.B;

        /// <inheritdoc />
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
            Coordinates = new Coordinates3D(x, y, z);
        }

        /// <summary>
        ///     Creates new cartesian vector from 3D coordinate tuple
        /// </summary>
        /// <param name="coordinates"></param>
        public Cartesian3D(in Coordinates3D coordinates)
            : this()
        {
            Coordinates = coordinates;
        }

        /// <summary>
        ///     Checks if three cartesian vectors form a linear independent system
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="vecB"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool IsLinearIndependentFrom(in Cartesian3D vecA, in Cartesian3D vecB, IEqualityComparer<double> comparer) =>
            Coordinates.IsLinearIndependentFrom(vecA.Coordinates, vecB.Coordinates, comparer);

        /// <summary>
        ///     Checks if the cartesian vector is linear independent from another
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool IsLinearIndependentFrom(in Cartesian3D vecA, IEqualityComparer<double> comparer) =>
            Coordinates.IsLinearIndependentFrom(vecA.Coordinates, comparer);

        /// <summary>
        ///     Vector addition
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Cartesian3D operator +(Cartesian3D lhs, Cartesian3D rhs) => new Cartesian3D(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);

        /// <summary>
        ///     Vector subtraction
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Cartesian3D operator -(Cartesian3D lhs, Cartesian3D rhs) => new Cartesian3D(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);

        /// <summary>
        ///     Vector addition
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Cartesian3D operator +(Cartesian3D lhs, ICartesian3D rhs) => new Cartesian3D(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);

        /// <summary>
        ///     Vector subtraction
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Cartesian3D operator -(Cartesian3D lhs, ICartesian3D rhs) => new Cartesian3D(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);

        /// <summary>
        ///     Scalar product of two cartesian vectors
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static double operator *(Cartesian3D lhs, Cartesian3D rhs) => lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;

        /// <summary>
        ///     Multiplication with scalar value
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Cartesian3D operator *(Cartesian3D lhs, double scalar) => new Cartesian3D(lhs.X * scalar, lhs.Y * scalar, lhs.Z * scalar);

        /// <summary>
        ///     Division by scalar value
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Cartesian3D operator /(Cartesian3D lhs, double scalar) => lhs * (1.0 / scalar);

        /// <summary>
        ///     Get the length of the vector
        /// </summary>
        /// <returns></returns>
        public double GetLength() => Math.Sqrt(X * X + Y * Y + Z * Z);

        /// <summary>
        ///     Get a <see cref="Cartesian3D" /> where each component is squared
        /// </summary>
        /// <returns></returns>
        public Cartesian3D GetSquared() => new Cartesian3D(X * X, Y * Y, Z * Z);

        /// <inheritdoc />
        public override string ToString() => $"Cartesian3D ({X}, {Y}, {Z})";
    }
}