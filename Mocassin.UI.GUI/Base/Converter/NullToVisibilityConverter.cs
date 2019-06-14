using System;
using System.Globalization;
using System.Windows;

namespace Mocassin.UI.GUI.Base.Converter
{
    /// <summary>
    ///     A <see cref="ValueConverter" /> that returns <see cref="Visibility.Visible" /> only if the passed value is not null and not a unset value
    /// </summary>
    public class NullToVisibilityConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || value == DependencyProperty.UnsetValue ? Visibility.Hidden : Visibility.Visible;
        }
    }
}