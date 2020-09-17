using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;
using Mocassin.UI.GUI.Base.Converter;

namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     A <see cref="ValueConverter" /> to convert color properties into the actual colo
    /// </summary>
    public class ColorToPropertyConverter : ValueConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Color color)) return null;
            if (!(parameter is ObjectDataProvider objectDataProvider)) return null;
            if (!(objectDataProvider.Data is IEnumerable<PropertyInfo> properties)) return null;

            foreach (var property in properties)
            {
                var other = (Color) property.GetValue(null);
                if (other.Equals(color)) return property;
            }

            return null;
        }

        /// <inheritdoc />
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is PropertyInfo propertyInfo)) return Colors.Black;
            return !(parameter is ObjectDataProvider) ? null : propertyInfo.GetValue(null);
        }
    }
}