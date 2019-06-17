using System;
using System.Globalization;
using NCalc;

namespace Mocassin.UI.GUI.Base.Converter
{
    /// <summary>
    ///     A <see cref="ValueConverter" /> that converts numeric <see cref="object" /> into <see cref="string" /> values with
    ///     backward conversion trough NCalc expression interpreter
    /// </summary>
    public class NumericExpressionToValueConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IConvertible convertible) return convertible.ToString(culture);
            return value?.ToString();
        }

        /// <inheritdoc />
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value?.ToString() is string code)) return null;
            if (code == "null" || code == "Null") return null;
            try
            {
                return new Expression(code).Evaluate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}