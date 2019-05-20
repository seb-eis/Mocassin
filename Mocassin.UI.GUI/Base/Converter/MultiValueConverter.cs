using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Mocassin.UI.GUI.Base.Converter
{
    /// <summary>
    ///     Base class for all <see cref="IMultiValueConverter"/> implementations that are a <see cref="MarkupExtension"/>
    /// </summary>
    public abstract class MultiValueConverter : MarkupExtension, IMultiValueConverter
    {
        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <inheritdoc />
        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

        /// <inheritdoc />
        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Back conversion is not supported by this converter");
        }
    }
}