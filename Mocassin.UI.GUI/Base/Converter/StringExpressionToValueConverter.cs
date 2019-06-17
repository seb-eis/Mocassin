using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Mocassin.UI.GUI.Base.Converter
{
    /// <summary>
    ///     A <see cref="ValueConverter" /> that converts <see cref="object" /> into <see cref="string" /> values with backward
    ///     conversion trough CSharpScript interface (Warning: Memory leaks, loaded assemblies cannot be unloaded again)
    /// </summary>
    public class StringExpressionToValueConverter : ValueConverter
    {
        /// <summary>
        ///     Get the default <see cref="List{T}" /> of script assembly references
        /// </summary>
        public static List<Assembly> DefaultScriptAssemblies { get; }

        /// <summary>
        ///     Get the default <see cref="List{T}" /> of script imports
        /// </summary>
        public static List<string> DefaultScriptImports { get; }

        /// <summary>
        ///     Static constructor that performs a single script evaluation the the background
        /// </summary>
        static StringExpressionToValueConverter()
        {
            DefaultScriptAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic && x.FullName.Contains("mscorlib") || x.FullName.Contains("System.Core"))
                .ToList();

            DefaultScriptImports = new List<string> {"System", "System.Math", "System.Linq"};
        }

        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IConvertible convertible) return convertible.ToString(culture);
            return value?.ToString();
        }

        /// <inheritdoc />
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var evaluation = GetEvaluation(value as string, targetType);
            return evaluation.Invoke();
        }

        /// <summary>
        ///     Converts the body <see cref="string" /> into a return value <see cref="Func{TResult}" /> delegate or null if it
        ///     cannot be converted
        /// </summary>
        /// <param name="bodyString"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static Func<object> GetEvaluation(string bodyString, Type targetType)
        {
            if (bodyString == null) return null;

            try
            {
                var code = GetExpressionCode(bodyString, targetType);
                var options = GetScriptOptions(targetType);
                return CSharpScript.EvaluateAsync<Func<object>>(code, options).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return () => null;
            }
        }

        /// <summary>
        ///     Builds a lambda code <see cref="string" /> from the passed body
        /// </summary>
        /// <param name="rawBody"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static string GetExpressionCode(string rawBody, Type targetType)
        {
            if (rawBody == null || targetType == null) return "() => null";

            var codeBody = rawBody;
            if (codeBody.Contains("return") && rawBody.LastIndexOf(';') != rawBody.Length)
                codeBody = $"{{{codeBody};}}";

            return $"() => {codeBody}";
        }

        /// <summary>
        ///     Get the <see cref="ScriptOptions" /> for the code evaluation
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static ScriptOptions GetScriptOptions(Type targetType)
        {
            if (targetType == null) throw new ArgumentNullException(nameof(targetType));
            return ScriptOptions.Default.AddReferences(DefaultScriptAssemblies).AddImports(DefaultScriptImports);
        }
    }
}