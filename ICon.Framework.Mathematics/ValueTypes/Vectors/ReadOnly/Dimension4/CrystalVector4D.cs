using System;
using System.Xml.Serialization;

using ICon.Framework.Xml;
using ICon.Mathematics.Extensions;

namespace ICon.Mathematics.ValueTypes
{
    /// <summary>
    /// Four dimensional 128 bit encoded linear supercell crystal position information
    /// </summary>
    public readonly struct CrystalVector4D : ICrystalVector4D<CrystalVector4D>, IComparable<CrystalVector4D>, IEquatable<CrystalVector4D>
    {
        /// <summary>
        /// The null vector of this type
        /// </summary>
        public static CrystalVector4D NullVector { get; } = new CrystalVector4D(0, 0, 0, 0);

        /// <summary>
        /// The 128 bit coordinate tuple
        /// </summary>
        public Coordinates<int, int, int, int> Coordinates { get; }

        /// <summary>
        /// The offset in A direction
        /// </summary>
        public int A => Coordinates.A;

        /// <summary>
        /// The offset in B direction
        /// </summary>
        public int B => Coordinates.B;

        /// <summary>
        /// The offset in C direction
        /// </summary>
        public int C => Coordinates.C;

        /// <summary>
        /// The position ID ithin the unit cell
        /// </summary>
        public int P => Coordinates.D;

        /// <summary>
        /// Creates new 128 bit crsytal vector from integer cooridnates
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="p"></param>
        public CrystalVector4D(int a, int b, int c, int p) : this()
        {
            Coordinates = new Coordinates<int, int, int, int>(a,b,c,p);
        }

        /// <summary>
        /// Factory function to create a new vector by interface access
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public CrystalVector4D CreateNew(int a, int b, int c, int p)
        {
            return new CrystalVector4D(a, b, c, p);
        }

        /// <summary>
        /// Vector substraction of all values
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static CrystalVector4D operator -(CrystalVector4D lhs, CrystalVector4D rhs)
        {
            return new CrystalVector4D(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C, lhs.P - rhs.P);
        }

        /// <summary>
        /// Vector substraction of all values
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static CrystalVector4D operator -(CrystalVector4D lhs, ICrystalVector4D rhs)
        {
            return new CrystalVector4D(lhs.A - rhs.A, lhs.B - rhs.B, lhs.C - rhs.C, lhs.P - rhs.P);
        }

        /// <summary>
        /// Vector addition of all values
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static CrystalVector4D operator +(CrystalVector4D lhs, ICrystalVector4D rhs)
        {
            return new CrystalVector4D(lhs.A + rhs.A, lhs.B + rhs.B, lhs.C + rhs.C, lhs.P + rhs.P);
        }

        /// <summary>
        /// Returns crystal vector as string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Crystal Vector ({A}, {B}, {C}, {P})";
        }

        /// <summary>
        /// Compares to other crystal vector (Coordinates are compared in order a, b, c, p)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(CrystalVector4D other)
        {
            return Coordinates.CompareCoordinates(other.Coordinates);
        }

        /// <summary>
        /// Cechsk for equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CrystalVector4D other)
        {
            return Coordinates.CoordinatesEqual(other.Coordinates);
        }
    }
}
