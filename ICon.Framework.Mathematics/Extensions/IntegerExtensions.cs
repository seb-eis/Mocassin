using System;
using System.Globalization;

namespace Mocassin.Mathematics.Extensions
{
    /// <summary>
    ///     ICon integer extension class
    /// </summary>
    public static class IntegerExtensions
    {
        /// <summary>
        ///     Get true/false of a single bit
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool GetBit(this uint integer, int index) => (integer & (1U << index)) != 0U;

        /// <summary>
        ///     Set a single bit within an integer to true
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static uint SetBit(this uint integer, int index) => integer | (1U << index);

        /// <summary>
        ///     Set a single bit within an integer to false
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static uint UnsetBit(this uint integer, int index) => integer - (integer & (1U << index));

        /// <summary>
        ///     Sets all true bits of secoond integer to true in the first
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static uint SetBits(this uint integer, uint flags) => integer + (~integer & flags);

        /// <summary>
        ///     Sets all true bits in the second integer to false in the first
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static uint UnsetBits(this uint integer, uint flags) => integer - (integer & flags);

        /// <summary>
        ///     Get true/false of a single bit
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool GetBit(this ulong integer, int index) => (integer & (1U << index)) != 0U;

        /// <summary>
        ///     Set a single bit within an integer to true
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static ulong SetBit(this ulong integer, int index) => integer | (1U << index);

        /// <summary>
        ///     Set a single bit within an integer to false
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static ulong UnsetBit(this ulong integer, int index) => integer - (integer & (1U << index));

        /// <summary>
        ///     Sets all true bits of second integer to true in the first
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static ulong SetBits(this ulong integer, ulong flags) => integer + (~integer & flags);

        /// <summary>
        ///     Sets all true bits in the second integer to false in the first
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static ulong UnsetBits(this ulong integer, ulong flags) => integer - (integer & flags);

        /// <summary>
        ///     Performs a population count on an 64 bit unsigned integer
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int PopCount(this ulong value)
        {
            var result = value - ((value >> 1) & 0x5555555555555555UL);
            result = (result & 0x3333333333333333UL) + ((result >> 2) & 0x3333333333333333UL);
            return (int) (unchecked(((result + (result >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }

        /// <summary>
        ///     Performs a population count on an IConvertible struct (Will always result in the population count of the conversion
        ///     result to 64bit unsigned integer!)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int PopCount<T1>(this T1 value) where T1 : struct, IConvertible => value.ToUInt64(CultureInfo.InvariantCulture).PopCount();

        /// <summary>
        ///     Invert the byte order of a long value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long InvertBytes(this long value) =>
            ((value & 0xffL) << 56) |
            ((value & (0xffL << 8)) << 40) |
            ((value & (0xffL << 16)) << 24) |
            ((value & (0xffL << 24)) << 8) |
            ((value & (0xffL << 32)) >> 8) |
            ((value & (0xffL << 40)) >> 24) |
            ((value & (0xffL << 48)) >> 40) |
            ((value & (0xffL << 56)) >> 56);
    }
}