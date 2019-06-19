using System;
using System.Globalization;
using System.Windows;

namespace Mocassin.UI.GUI.Base.Converter
{
    /// <summary>
    ///     A <see cref="ValueConverter" /> that returns <see cref="Visibility.Visible" /> only if the passed value is not null
    ///     and not a unset value. The parameter allows manual definition of <see cref="Visibility" /> as a string
    /// </summary>
    public class NullToVisibilityConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = Enum.TryParse(parameter?.ToString(), out Visibility x) ? x : Visibility.Hidden;
            return value == null || value == DependencyProperty.UnsetValue ? visibility : Visibility.Visible;
        }
    }
}