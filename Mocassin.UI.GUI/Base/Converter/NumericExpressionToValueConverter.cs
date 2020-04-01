using System;
using System.Globalization;
using System.Text.RegularExpressions;
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
                if (value is IConvertible) return System.Convert.ChangeType(value, targetType, culture);
            }
            catch (Exception)
            {
                try
                {
                    var expression = PrepareExpression(code);
                    var converted = expression.Evaluate();
                    return targetType == typeof(string) ? converted?.ToString() : converted;
                }
                catch (Exception)
                {
                    Console.WriteLine($"The expression [{value}] does not evaluate to [{targetType}].");
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        ///     Prepares an new <see cref="Expression" /> using the provided <see cref="string" />
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static Expression PrepareExpression(string code)
        {
            code = FormatStringForExpressionParser(code);
            var expression = new Expression(code, EvaluateOptions.IgnoreCase);
            expression.EvaluateParameter += delegate(string name, ParameterArgs args)
            {
                if (Regex.Match(name, "^[Ee]{1}$").Success) args.Result = Math.E;
                if (Regex.Match(name, "^[Pp]{1}[Ii]{1}$").Success) args.Result = Math.PI;
            };
            expression.EvaluateFunction += delegate(string name, FunctionArgs args)
            {
                if (Regex.Match(name, "^[Ll][Nn]$").Success)
                {
                    var parameter = args.EvaluateParameters();
                    args.Result = Math.Log(System.Convert.ToDouble(parameter[0]), Math.E);
                }

                var logMatch = Regex.Match(name, "^[Ll][Oo][Gg]([0-9]+)$");
                if (logMatch.Success)
                {
                    var logBase = System.Convert.ToDouble(logMatch.Groups[1].Value);
                    var parameter = args.EvaluateParameters();
                    args.Result = Math.Log(System.Convert.ToDouble(parameter[0]), logBase);
                }

                var rootMatch = Regex.Match(name, "^[Rr][Oo]{2}[Tt]([0-9]+)$");
                if (!rootMatch.Success) return;
                {
                    var rootBase = System.Convert.ToDouble(rootMatch.Groups[1].Value);
                    var parameter = args.EvaluateParameters();
                    args.Result = Math.Pow(System.Convert.ToDouble(parameter[0]), 1.0 / rootBase);
                }
            };
            return expression;
        }

        /// <summary>
        ///     Prepares the raw code string for expression parsing
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string FormatStringForExpressionParser(string code)
        {
            var formattedCode = RemoveWhiteSpacesAndLeadingPlusSign(code);
            return formattedCode;
        }

        /// <summary>
        ///     Removes a leading "+" and existing white spaces sign from the string if existent, else returns the original
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string RemoveWhiteSpacesAndLeadingPlusSign(string code)
        {
            code = Regex.Replace(code, "\\s+", "");
            return code[0] != '+' ? code : code.Substring(1, code.Length - 1);
        }
    }
}