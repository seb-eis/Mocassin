using System;
using System.Globalization;

namespace ICon.Mathematics.Extensions
{
    /// <summary>
    /// ICon integer extension class
    /// </summary>
    public static class IntegerExtensions
    {
        /// <summary>
        /// Get true/false of a single bit
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Boolean GetBit(this UInt32 integer, Int32 index)
        {
            return (integer & (1U << index)) != 0U;
        }

        /// <summary>
        /// Set a single bit within an integer to true
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UInt32 SetBit(this UInt32 integer, Int32 index)
        {
            return integer | (1U << index);
        }

        /// <summary>
        /// Set a single bit within an integer to false
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static UInt32 UnsetBit(this UInt32 integer, Int32 index)
        {
            return integer - (integer & (1U << index));
        }

        /// <summary>
        /// Sets all true bits of secoond integer to true in the first
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static UInt32 SetBits(this UInt32 integer, UInt32 flags)
        {
            return integer + (~integer & flags);
        }

        /// <summary>
        /// Sets all true bits in the second integer to false in the first
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static UInt32 UnsetBits(this UInt32 integer, UInt32 flags)
        {
            return integer - (integer & flags);
        }

        /// <summary>
        /// Get true/false of a single bit
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Boolean GetBit(this UInt64 integer, Int32 index)
        {
            return (integer & (1U << index)) != 0U;
        }

        /// <summary>
        /// Set a single bit within an integer to true
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UInt64 SetBit(this UInt64 integer, Int32 index)
        {
            return integer | (1U << index);
        }

        /// <summary>
        /// Set a single bit within an integer to false
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static UInt64 UnsetBit(this UInt64 integer, Int32 index)
        {
            return integer - (integer & (1U << index));
        }

        /// <summary>
        /// Sets all true bits of secoond integer to true in the first
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static UInt64 SetBits(this UInt64 integer, UInt64 flags)
        {
            return integer + (~integer & flags);
        }

        /// <summary>
        /// Sets all true bits in the second integer to false in the first
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static UInt64 UnsetBits(this UInt64 integer, UInt64 flags)
        {
            return integer - (integer & flags);
        }

        /// <summary>
        /// Performs a population count on an 64 bit unsigned integer
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int32 PopCount(this UInt64 value)
        {
            UInt64 result = value - ((value >> 1) & 0x5555555555555555UL);
            result = (result & 0x3333333333333333UL) + ((result >> 2) & 0x3333333333333333UL);
            return (Int32)(unchecked(((result + (result >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }

        /// <summary>
        /// Performs a population count on an IConvertible struct (Will always result in the population count of the conversion result to 64bit unsigned integer!)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int32 PopCount<T1>(this T1 value) where T1 : struct, IConvertible
        {
            return value.ToUInt64(CultureInfo.InvariantCulture).PopCount();
        }
    }
}
