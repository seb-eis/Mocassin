using System;
using System.Globalization;

namespace Mocassin.UI.GUI.Base.Converter
{
    /// <summary>
    ///     <see cref="ValueConverter"/> that converts from <see cref="string"/> to an <see cref="Enum"/> member
    /// </summary>
    public class EnumToStringConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        /// <inheritdoc />
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value?.ToString() is { } enumString)) return null;
            try
            {
                var enumType = targetType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? Nullable.GetUnderlyingType(targetType)
                    : targetType;
                return enumType == null ? null : Enum.Parse(enumType, enumString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}