using System;

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
    }
}