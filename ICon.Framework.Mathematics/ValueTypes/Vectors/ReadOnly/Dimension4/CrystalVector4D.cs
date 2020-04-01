using System;
using System.Collections.Generic;
using Moccasin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Four dimensional 128 bit encoded linear supercell crystal position information
    /// </summary>
    public readonly struct CrystalVector4D : ICrystalVector4D, IComparable<CrystalVector4D>, IEquatable<CrystalVector4D>
    {
        /// <summary>
        ///     The null vector of this type
        /// </summary>
        public static CrystalVector4D NullVector { get; } = new CrystalVector4D(0, 0, 0, 0);

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
        public CrystalVector4D(int a, int b, int c, int p)
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
        public static CrystalVector4D operator -(CrystalVector4D lhs, ICrystalVector4D rhs)
        {
            return new CrystalVector4D(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C, lhs.P - rhs.P);
        }

        /// <summary>
        ///     Vector addition of all values
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static CrystalVector4D operator +(CrystalVector4D lhs, ICrystalVector4D rhs)
        {
            return new CrystalVector4D(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C, lhs.P + rhs.P);
        }

        /// <summary>
        ///     Vector multiplication with scalar value
        /// </summary>
        public static CrystalVector4D operator *(CrystalVector4D lhs, int scalar)
        {
            return new CrystalVector4D(lhs.A * scalar, lhs.B * scalar, lhs.C * scalar, lhs.P * scalar);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"CrystalVector ({A}, {B}, {C}, {P})";
        }

        /// <inheritdoc />
        public int CompareTo(CrystalVector4D other)
        {
            return Coordinates.CompareTo(other.Coordinates);
        }

        /// <inheritdoc />
        public bool Equals(CrystalVector4D other)
        {
            return Coordinates.Equals(other.Coordinates);
        }

        /// <summary>
        ///     Get a <see cref="CrystalVector4D" /> trimmed back into a super-cell with periodic boundary sizes
        /// </summary>
        /// <param name="sizes"></param>
        /// <returns></returns>
        public CrystalVector4D ToTrimmed(in VectorI3 sizes)
        {
            return ToTrimmed(sizes.A, sizes.B, sizes.C);
        }

        /// <summary>
        ///     Get a <see cref="CrystalVector4D" /> trimmed back into a super-cell with periodic boundary sizes
        /// </summary>
        /// <param name="sizes"></param>
        /// <returns></returns>
        public CrystalVector4D ToTrimmed(in CrystalVector4D sizes)
        {
            return ToTrimmed(sizes.A, sizes.B, sizes.C);
        }

        /// <summary>
        ///     Get a <see cref="CrystalVector4D" /> trimmed back into a super-cell with periodic boundary sizes
        /// </summary>
        /// <param name="maxA"></param>
        /// <param name="maxB"></param>
        /// <param name="maxC"></param>
        /// <returns></returns>
        public CrystalVector4D ToTrimmed(int maxA, int maxB, int maxC)
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
            return new CrystalVector4D(a, b, c, P);
        }

        /// <summary>
        ///     Converts the <see cref="CrystalVector4D" /> to a linear integer index using the provided
        ///     <see cref="CrystalVector4D" /> super-cell size data
        /// </summary>
        /// <param name="sizes"></param>
        /// <returns></returns>
        public int ToLinearIndex(in CrystalVector4D sizes)
        {
            return P +
                   C * sizes.P +
                   B * sizes.P * sizes.C +
                   A * sizes.P * sizes.C * sizes.B;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(A, B, C, P);
        }

        /// <summary>
        ///     Enumerates all possible <see cref="CrystalVector4D" /> for a super-cell for the defined
        ///     <see cref="CrystalVector4D" /> size data
        /// </summary>
        /// <param name="sizes"></param>
        /// <returns></returns>
        public static IEnumerable<CrystalVector4D> LatticeVectorSet(CrystalVector4D sizes)
        {
            for (var a = 0; a < sizes.A; a++)
            {
                for (var b = 0; b < sizes.B; b++)
                {
                    for (var c = 0; c < sizes.C; c++)
                    {
                        for (var p = 0; p < sizes.P; p++) yield return new CrystalVector4D(a, b, c, p);
                    }
                }
            }
        }
    }
}