using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Mocassin.UI.GUI.Base.Converter
{
    /// <summary>
    ///     Base class for typed <see cref="IValueConverter"/> implementations
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TParam"></typeparam>
    public abstract class ValueConverter<TIn, TOut, TParam> : MarkupExtension, IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TIn)) return value;
            return Convert((TIn) value, (TParam) parameter, culture);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((TOut) value, (TParam) parameter, culture);
        }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <summary>
        ///     Typed implementation of the value-to-value conversion for <see cref="TOut"/> to <see cref="TIn"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public virtual TIn ConvertBack(TOut value, TParam parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Back conversion is not supported by this converter");
        }

        /// <summary>
        ///     Typed implementation of the value-to-value conversion for <see cref="TIn"/> to <see cref="TOut"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract TOut Convert(TIn value, TParam parameter, CultureInfo culture);
    }
}