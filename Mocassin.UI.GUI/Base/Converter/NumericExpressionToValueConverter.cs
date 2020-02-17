using System;
using System.Globalization;
using NCalc;

namespace Mocassin.UI.GUI.Base.Converter
{
    /// <summary>
    ///     A <see cref="ValueConverter" /> that converts numeric <see cref="string" /> expressions into <see cref="string" />
    ///     values with
    ///     backward conversion trough NCalc expression interpreter
    /// </summary>
    public class NumericExpressionToValueConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        /// <inheritdoc />
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value?.ToString() is { } code)) return null;
            if (code == "null" || code == "Null") return null;
            try
            {
                var converted = new Expression(RemoveLeadingPlusSign(code)).Evaluate();
                return targetType == typeof(string) ? converted?.ToString() : converted;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        /// <summary>
        ///     Removes a leading "+" sign from the string if existent, else returns the original
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string RemoveLeadingPlusSign(string code)
        {
            return code[0] != '+' ? code : code.Substring(1, code.Length - 1);
        }
    }
}