﻿using System;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Mathematics.Bitmask
{
    /// <summary>
    ///     Wrapped 64 bit unsigned integer to be used as 64 entry bitmask (Waring: Mutable struct!)
    /// </summary>
    public struct Bitmask64 : IBitmask, IComparable<Bitmask64>, IEquatable<Bitmask64>
    {
        /// <summary>
        ///     The internal mask value
        /// </summary>
        public ulong Mask { get; set; }

        /// <summary>
        ///     Get the size of the mask
        /// </summary>
        public int MaxEntries => 64;

        /// <summary>
        ///     Creates new mask from unsigned integer
        /// </summary>
        /// <param name="mask"></param>
        public Bitmask64(ulong mask)
        {
            Mask = mask;
        }

        /// <summary>
        ///     Copy constructor
        /// </summary>
        /// <param name="bitmask"></param>
        public Bitmask64(Bitmask64 bitmask)
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
        ///     Compares the wrapped integer
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Bitmask64 other) => Mask.CompareTo(other.Mask);

        /// <inheritdoc />
        public bool Equals(Bitmask64 other) => Mask == other.Mask;

        /// <summary>
        ///     Counts the number of set bits
        /// </summary>
        /// <returns></returns>
        public int PopCount() => Mask.PopCount();


        /// <summary>
        ///     Implicit conversion operator for unsigned 64 bit integer
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Bitmask64(ulong value) => new Bitmask64(value);

        /// <summary>
        ///     String representation fo the bitmask
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Bitmask ({Mask.ToString()})";
    }
}