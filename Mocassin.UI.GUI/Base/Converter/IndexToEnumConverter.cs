using System;
using System.Globalization;

namespace Mocassin.UI.GUI.Base.Converter
{
    /// <summary>
    ///     <see cref="ValueConverter" /> implementation to convert indexing values to <see cref="System.Enum" /> instances
    /// </summary>
    public class IndexToEnumConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Type type && value is int integer) return Enum.ToObject(type, integer);
            return null;
        }
    }
}