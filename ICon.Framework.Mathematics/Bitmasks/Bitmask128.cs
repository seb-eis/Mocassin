using System;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Mathematics.Bitmask
{
    /// <summary>
    ///     Two wrapped 64 bit unsigned integer to be used as 128 entry bitmask (Waring: Mutable struct!)
    /// </summary>
    public class Bitmask128 : IBitmask, IComparable<Bitmask128>, IEquatable<Bitmask128>
    {
        /// <summary>
        ///     The first internal 64 bit mask
        /// </summary>
        public ulong Mask0 { get; set; }

        /// <summary>
        ///     The first internal 64 bit mask
        /// </summary>
        public ulong Mask1 { get; set; }

        /// <inheritdoc />
        public bool this[int index]
        {
            get => index < 64 ? Mask0.GetBit(index) : Mask1.GetBit(index - 64);
            set
            {
                if (value)
                {
                    if (index < 64)
                        Mask0 = Mask0.SetBit(index);
                    else
                        Mask1 = Mask1.SetBit(index - 64);
                }
                else
                {
                    if (index < 64)
                        Mask0 = Mask0.UnsetBit(index);
                    else
                        Mask1 = Mask1.UnsetBit(index - 64);
                }
            }
        }

        /// <summary>
        ///     Creates new mask from unsigned integer
        /// </summary>
        /// <param name="mask0"></param>
        /// <param name="mask1"></param>
        public Bitmask128(ulong mask0, ulong mask1)
        {
            Mask0 = mask0;
            Mask1 = mask1;
        }

        /// <summary>
        ///     Copy constructor
        /// </summary>
        /// <param name="bitmask"></param>
        public Bitmask128(Bitmask128 bitmask)
        {
            Mask0 = bitmask.Mask0;
            Mask1 = bitmask.Mask1;
        }

        /// <summary>
        ///     Compares the wrapped integer
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Bitmask128 other)
        {
            var compare1 = Mask1.CompareTo(other.Mask1);
            return compare1 == 0
                ? Mask0.CompareTo(other.Mask0)
                : compare1;
        }

        /// <inheritdoc />
        public bool Equals(Bitmask128 other)
        {
            return other != null && Mask0 == other.Mask0 && Mask1 == other.Mask1;
        }
    }
}