using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     Provides extension methods for visual information
    /// </summary>
    public static class VisualExtensions
    {
        /// <summary>
        ///     Get the <see cref="Regex"/> to match the contents of the ARGB color format #FFFFFFFF
        /// </summary>
        public static Regex ColorRegexArgbHex { get; } = 
            new Regex(@"#(?<a>[a-fA-F0-9]{2})(?<r>[a-fA-F0-9]{2})(?<g>[a-fA-F0-9]{2})(?<b>[a-fA-F0-9]{2})");

        /// <summary>
        ///     Converts a <see cref="Color"/> to a ARGB #FFFFFFFF format
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToArgbHex(this Color color)
        {
            return color.ToString();
        }

        /// <summary>
        ///     Tries to convert a <see cref="string"/> to a <see cref="Color"/> if in the  ARGB #FFFFFFFF format
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool TryParseArgbHex(string str, out Color color)
        {
            if (str is null)
            {
                color = default;
                return false;
            }

            var match = ColorRegexArgbHex.Match(str);
            if (!match.Success)
            {
                color = default;
                return false;
            }

            const NumberStyles numberStyle = System.Globalization.NumberStyles.HexNumber;

            var a = byte.Parse(match.Groups["a"].Value, numberStyle);
            var r = byte.Parse(match.Groups["r"].Value, numberStyle);
            var g = byte.Parse(match.Groups["g"].Value, numberStyle);
            var b = byte.Parse(match.Groups["b"].Value, numberStyle);
            color = Color.FromArgb(a, r, g, b);
            return true;
        }

        /// <summary>
        ///     Converts a <see cref="string" /> to a <see cref="Color" /> if in the ARGB #FFFFFFFF format (With option to suppress conversion errors)
        ///     use a default over an exception on parsing error
        /// </summary>
        /// <param name="str"></param>
        /// <param name="noException"></param>
        /// <returns></returns>
        public static Color ParseArgbHex(string str, bool noException = true)
        {
            if (TryParseArgbHex(str, out var color)) return color;
            if (noException) return Color.FromArgb(byte.MaxValue, 0, 0, 0);
            throw new FormatException("Invalid color format.");
        }
    }
}