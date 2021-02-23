using System;
using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Four dimensional 128 bit encoded linear supercell crystal position information
    /// </summary>
    public readonly struct Vector4I : IVector4I, IComparable<Vector4I>, IEquatable<Vector4I>
    {
        /// <summary>
        ///     The null vector of this type
        /// </summary>
        public static Vector4I Zero { get; } = new Vector4I(0, 0, 0, 0);

        /// <summary>
        ///     The integer coordinate tuple
        /// </summary>
        public Coordinates4I Coordinates { get; }

        /// <inheritdoc />
        public int A => Coordinates.A;

        /// <inheritdoc />
        public int B => Coordinates.B;

        /// <inheritdoc />
        public int C => Coordinates.C;

        /// <inheritdoc />
        public int P => Coordinates.D;

        /// <summary>
        ///     Creates new from integer coordinates
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="p"></param>
        public Vector4I(int a, int b, int c, int p)
            : this()
        {
            Coordinates = new Coordinates4I(a, b, c, p);
        }

        /// <summary>
        ///     Vector subtraction of all values
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector4I operator -(Vector4I lhs, IVector4I rhs) => new Vector4I(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C, lhs.P - rhs.P);

        /// <summary>
        ///     Vector addition of all values
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector4I operator +(Vector4I lhs, IVector4I rhs) => new Vector4I(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C, lhs.P + rhs.P);

        /// <summary>
        ///     Vector multiplication with scalar value
        /// </summary>
        public static Vector4I operator *(Vector4I lhs, int scalar) => new Vector4I(lhs.A * scalar, lhs.B * scalar, lhs.C * scalar, lhs.P * scalar);

        /// <inheritdoc />
        public override string ToString() => $"CrystalVector ({A}, {B}, {C}, {P})";

        /// <inheritdoc />
        public int CompareTo(Vector4I other) => Coordinates.CompareTo(other.Coordinates);

        /// <inheritdoc />
        public bool Equals(Vector4I other) => Coordinates.Equals(other.Coordinates);

        /// <summary>
        ///     Get a <see cref="Vector4I" /> trimmed back into a super-cell with periodic boundary sizes
        /// </summary>
        /// <param name="sizes"></param>
        /// <returns></returns>
        public Vector4I ToTrimmed(in VectorI3 sizes) => ToTrimmed(sizes.A, sizes.B, sizes.C);

        /// <summary>
        ///     Get a <see cref="Vector4I" /> trimmed back into a super-cell with periodic boundary sizes
        /// </summary>
        /// <param name="sizes"></param>
        /// <returns></returns>
        public Vector4I ToTrimmed(in Vector4I sizes) => ToTrimmed(sizes.A, sizes.B, sizes.C);

        /// <summary>
        ///     Get a <see cref="Vector4I" /> trimmed back into a super-cell with periodic boundary sizes
        /// </summary>
        /// <param name="maxA"></param>
        /// <param name="maxB"></param>
        /// <param name="maxC"></param>
        /// <returns></returns>
        public Vector4I ToTrimmed(int maxA, int maxB, int maxC)
        {
            var a = A;
            var b = B;
            var c = C;
            while (a < 0) a += maxA;
            while (a >= maxA) a -= maxA;
            while (b < 0) b += maxB;
            while (b >= maxB) b -= maxB;
            while (c < 0) c += maxC;
            while (c >= maxC) c -= maxC;
            return new Vector4I(a, b, c, P);
        }

        /// <summary>
        ///     Converts the <see cref="Vector4I" /> to a linear integer index using the provided
        ///     <see cref="Vector4I" /> super-cell size data
        /// </summary>
        /// <param name="sizes"></param>
        /// <returns></returns>
        public int ToLinearIndex(in Vector4I sizes) =>
            P +
            C * sizes.P +
            B * sizes.P * sizes.C +
            A * sizes.P * sizes.C * sizes.B;

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(A, B, C, P);

        /// <summary>
        ///     Enumerates all possible <see cref="Vector4I" /> for a super-cell for the defined
        ///     <see cref="Vector4I" /> size data
        /// </summary>
        /// <param name="sizes"></param>
        /// <returns></returns>
        public static IEnumerable<Vector4I> LatticeVectorSet(Vector4I sizes)
        {
            for (var a = 0; a < sizes.A; a++)
            {
                for (var b = 0; b < sizes.B; b++)
                {
                    for (var c = 0; c < sizes.C; c++)
                    {
                        for (var p = 0; p < sizes.P; p++) yield return new Vector4I(a, b, c, p);
                    }
                }
            }
        }
    }
}