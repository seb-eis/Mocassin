using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Mocassin.UI.GUI.Base.Converter
{
    /// <summary>
    ///     A <see cref="ValueConverter" /> markup extension that converts between <see cref="string" /> values and
    ///     <see cref="Nullable" /> types if the underlying type implements a static parse function
    /// </summary>
    public class StringToNullableConverter : ValueConverter
    {
        private delegate T ParseDelegate<out T>(string str, IFormatProvider provider);

        /// <summary>
        ///     <see cref="Dictionary{TKey,TValue}" /> to cache already created parsing delegates
        /// </summary>
        private static Dictionary<Type, Delegate> ParsingDictionary { get; } = new Dictionary<Type, Delegate>();

        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        /// <inheritdoc />
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && string.IsNullOrWhiteSpace(str)) return null;
            try
            {
                return GetParseFunction(targetType)?.DynamicInvoke(value as string, culture);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        /// <summary>
        ///     Finds the required parse function for the provided target <see cref="Type" /> or creates a new cache entry if none
        ///     exists
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static Delegate GetParseFunction(Type targetType)
        {
            if (ParsingDictionary.TryGetValue(targetType, out var function)) return function;

            try
            {
                var underlyingType = Nullable.GetUnderlyingType(targetType);
                var method = underlyingType?.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null,
                    CallingConventions.Standard, new[] {typeof(string), typeof(IFormatProvider)}, null);
                var delegateType = typeof(ParseDelegate<>).MakeGenericType(underlyingType);
                ParsingDictionary[targetType] = method?.CreateDelegate(delegateType);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                ParsingDictionary[targetType] = null;
            }

            return ParsingDictionary[targetType];
        }
    }
}