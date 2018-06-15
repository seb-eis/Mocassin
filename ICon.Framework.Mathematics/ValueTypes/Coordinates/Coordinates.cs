using System;
using System.Xml.Serialization;

using ICon.Framework.Xml;
using ICon.Framework.Extensions;

namespace ICon.Mathematics.ValueTypes
{
    /// <summary>
    /// Interface that identifies a struct as a coordinate tuple
    /// </summary>
    public interface ICoordinates
    {
        /// <summary>
        /// Coordinate tuple size
        /// </summary>
        int Size { get; }
    }

    /// <summary>
    /// 1D Coordinate tuple
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public readonly struct Coordinates<T1> : ICoordinates where T1 : struct
    {
        /// <summary>
        /// The size information of the coordinate tuple
        /// </summary>
        private static int CoorTupleSize = 1;

        /// <summary>
        /// Create new coordinates
        /// </summary>
        /// <param name="a"></param>
        public Coordinates(T1 a) : this()
        {
            A = a;
        }

        /// <summary>
        /// First coordinate value
        /// </summary>
        public T1 A { get; }

        /// <summary>
        /// Access size info of the coordinate tuple
        /// </summary>
        public int Size => CoorTupleSize;
    }

    /// <summary>
    /// 2D Coordinate tuple
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public readonly struct Coordinates<T1, T2> : ICoordinates
        where T1 : struct 
        where T2 : struct
    {
        /// <summary>
        /// The size information of the coordinate tuple
        /// </summary>
        private static int CoorTupleSize = 2;

        /// <summary>
        /// Create new coordinates
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Coordinates(T1 a, T2 b) : this()
        {
            A = a;
            B = b;
        }

        /// <summary>
        /// First coordinate value
        /// </summary>
        public T1 A { get; }

        /// <summary>
        /// Second coordinate value
        /// </summary>
        public T2 B { get; }

        /// <summary>
        /// Access size info of the coordinate tuple
        /// </summary>
        [XmlIgnore]
        public int Size => CoorTupleSize;
    }

    /// <summary>
    /// 3D coordinate tuple
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public struct Coordinates<T1, T2, T3> : ICoordinates
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        /// <summary>
        /// The size information of the coordinate tuple
        /// </summary>
        private static int CoorTupleSize = 3;

        /// <summary>
        /// Create new coordinates
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Coordinates(T1 a, T2 b, T3 c) : this()
        {
            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        /// First coordinate value
        /// </summary>
        public T1 A { get; }

        /// <summary>
        /// Second coordinate value
        /// </summary>
        public T2 B { get; }

        /// <summary>
        /// Third coordinate value
        /// </summary>
        public T3 C { get; }

        /// <summary>
        /// Access size info of the coordinate tuple
        /// </summary>
        public int Size => CoorTupleSize;
    }

    /// <summary>
    /// 4D coordinate tuple
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public readonly struct Coordinates<T1, T2, T3, T4> : ICoordinates
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
    {
        /// <summary>
        /// The size information of the coordinate tuple
        /// </summary>
        private const int CoorTupleSize = 4;

        /// <summary>
        /// Create new coordinates
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public Coordinates(T1 a, T2 b, T3 c, T4 d) : this()
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        /// <summary>
        /// First coordinate value
        /// </summary>
        public T1 A { get; }

        /// <summary>
        /// Second coordinate value
        /// </summary>
        public T2 B { get; }

        /// <summary>
        /// Third coordinate value
        /// </summary>
        public T3 C { get; }

        /// <summary>
        /// Fourth coordinate value
        /// </summary>
        public T4 D { get; }

        /// <summary>
        /// Access size info of the coordinate tuple
        /// </summary>
        [XmlIgnore]
        public int Size => CoorTupleSize;
    }

    /// <summary>
    /// 5D coordinate tuple
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    public struct Coordinates<T1, T2, T3, T4, T5> : ICoordinates
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
    {
        /// <summary>
        /// The size information of the coordinate tuple
        /// </summary>
        private const int CoorTupleSize = 5;

        /// <summary>
        /// Create new coordinates
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public Coordinates(T1 a, T2 b, T3 c, T4 d, T5 e) : this()
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
        }

        /// <summary>
        /// First coordinate value
        /// </summary>
        public T1 A { get; }

        /// <summary>
        /// Second coordinate value
        /// </summary>
        public T2 B { get; }

        /// <summary>
        /// Third coordinate value
        /// </summary>
        public T3 C { get; }

        /// <summary>
        /// Fourth coordinate value
        /// </summary>
        public T4 D { get; }

        /// <summary>
        /// Fith coordinate value
        /// </summary>
        public T5 E { get; }

        /// <summary>
        /// Access size info of the coordinate tuple
        /// </summary>
        public int Size => CoorTupleSize;
    }
}
