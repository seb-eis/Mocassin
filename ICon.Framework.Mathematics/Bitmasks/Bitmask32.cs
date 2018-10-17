using System;
using System.Runtime.Serialization;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Mathematics.Bitmasks
{
    /// <summary>
    ///     Wrapped 32 bit unsigned integer to be used as 32 entry bitmask (Waring: Mutable struct!)
    /// </summary>
    [DataContract]
    public struct Bitmask32 : IBitmask, IComparable<Bitmask32>, IEquatable<Bitmask32>
    {
        /// <summary>
        ///     The internal mask value
        /// </summary>
        [DataMember]
        public uint Mask { get; set; }

        /// <summary>
        ///     Creates new mask from unsigned integer
        /// </summary>
        /// <param name="mask"></param>
        public Bitmask32(uint mask)
        {
            Mask = mask;
        }

        /// <summary>
        ///     Copy constructor
        /// </summary>
        /// <param name="bitmask"></param>
        public Bitmask32(Bitmask32 bitmask)
        {
            Mask = bitmask.Mask;
        }

        /// <inheritdoc />
        public bool this[int index]
        {
            get => Mask.GetBit(index);
            set => Mask = value
                ? Mask.SetBit(index)
                : Mask.UnsetBit(index);
        }

        /// <summary>
        ///     Implicit conversion operator for unsigned 32 bit integer
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Bitmask32(uint value)
        {
            return new Bitmask32(value);
        }

        /// <summary>
        ///     Compares the wrapped integer
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Bitmask32 other)
        {
            return Mask.CompareTo(other.Mask);
        }

        /// <inheritdoc />
        public bool Equals(Bitmask32 other)
        {
            return Mask == other.Mask;
        }
    }
}