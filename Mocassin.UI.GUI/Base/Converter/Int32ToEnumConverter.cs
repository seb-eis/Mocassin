using System;
using System.Globalization;

namespace Mocassin.UI.GUI.Base.Converter
{
    /// <summary>
    ///     <see cref="ValueConverter" /> implementation to convert <see cref="int" /> values to <see cref="System.Enum" />
    ///     instances
    /// </summary>
    public class Int32ToEnumConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Type type && value is int integer) return Enum.ToObject(type, integer);
            return null;
        }
    }
}