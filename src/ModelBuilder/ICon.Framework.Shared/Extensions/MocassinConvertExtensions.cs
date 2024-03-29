﻿using System;
using System.Globalization;

namespace Mocassin.Framework.Extensions
{
    /// <summary>
    ///     Provides base type value conversions as extension methods for types implementing IConvertible (Only compatible with
    ///     primitive types)
    /// </summary>
    public static class MocassinConvertExtensions
    {
        /// <summary>
        ///     Converts a string to a primitive type using invariant culture format provider (Throws if type is not primitive)
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="literal"></param>
        /// <returns></returns>
        public static TTarget ChangeType<TTarget>(this string literal) where TTarget : IConvertible =>
            (TTarget) Convert.ChangeType(literal, typeof(TTarget), CultureInfo.InvariantCulture);

        /// <summary>
        ///     Converts a string to a primitive type using the provided format provider (Throws if type is not primitive)
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="literal"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static TTarget ChangeType<TTarget>(this string literal, IFormatProvider provider) where TTarget : IConvertible =>
            (TTarget) Convert.ChangeType(literal, typeof(TTarget), provider);

        /// <summary>
        ///     Converts a primitive type to another primitive type using the invariant culture format provider (Throws if type is
        ///     not primitive)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TTarget ChangeType<TSource, TTarget>(this TSource value) where TSource : IConvertible where TTarget : IConvertible =>
            (TTarget) Convert.ChangeType(value, typeof(TTarget), CultureInfo.InvariantCulture);

        /// <summary>
        ///     Converts a primitive type to another primitive type using the provided format provider (Throws if type is not
        ///     primitive)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static TTarget ChangeType<TSource, TTarget>(this TSource value, IFormatProvider provider)
            where TSource : IConvertible where TTarget : IConvertible =>
            (TTarget) Convert.ChangeType(value, typeof(TTarget), provider);

        /// <summary>
        ///     Converts a primitive type to a string using the culture invariant format provider (Throws if type is not primitive)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ChangeToString<TSource>(this TSource value) where TSource : IConvertible =>
            (string) Convert.ChangeType(value, typeof(string), CultureInfo.InvariantCulture);

        /// <summary>
        ///     Converts a primitive type to a string using the provided format provider (Throws if type is not primitive)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="value"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public static string ChangeToString<TSource>(this TSource value, IFormatProvider formatProvider) where TSource : IConvertible =>
            (string) Convert.ChangeType(value, typeof(string), formatProvider);
    }
}