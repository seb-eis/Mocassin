using System;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Represents a context free, readonly 16 byte coordinate information for 4D translation invariant integer space
    /// </summary>
    public readonly struct Coordinates4I : ICoordinates, IComparable<Coordinates4I>, IEquatable<Coordinates4I>
    {
        /// <summary>
        ///     First coordinate value
        /// </summary>
        public int A { get; }

        /// <summary>
        ///     Second coordinate value
        /// </summary>
        public int B { get; }

        /// <summary>
        ///     Third coordinate value
        /// </summary>
        public int C { get; }

        /// <summary>
        ///     Fourth coordinate value
        /// </summary>
        public int D { get; }

        /// <inheritdoc />
        public int Dimension => 4;

        /// <summary>
        ///     Creates a new <see cref="Coordinates4I" /> struct from coordinate points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public Coordinates4I(int a, int b, int c, int d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        /// <inheritdoc />
        public int CompareTo(Coordinates4I other)
        {
            var compA = A.CompareTo(other.A);
            if (compA != 0) return compA;

            var compB = B.CompareTo(other.B);
            if (compB != 0) return compB;

            var compC = C.CompareTo(other.C);
            return compC != 0 ? compC : D.CompareTo(other.D);
        }

        /// <inheritdoc />
        public bool Equals(Coordinates4I other)
        {
            return CompareTo(other) == 0;
        }
    }
}