using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Microsoft.EntityFrameworkCore.Design;

namespace Mocassin.UI.GUI.Base.Converter
{
    public class IndexToEnumConverter : MarkupExtension, IValueConverter
    {
        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Type type && value is int integer) return Enum.ToObject(type, integer);
            return null;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}