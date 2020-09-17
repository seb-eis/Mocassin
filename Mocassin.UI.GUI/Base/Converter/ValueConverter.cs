using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Mocassin.UI.GUI.Base.Converter
{
    /// <summary>
    ///     Base class for all <see cref="IValueConverter" /> implementations that are a <see cref="MarkupExtension" />
    /// </summary>
    public abstract class ValueConverter : MarkupExtension, IValueConverter
    {
        /// <inheritdoc />
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <inheritdoc />
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException("Back conversion is not supported by this converter");

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}