using System;
using System.Globalization;
using System.Windows.Data;

namespace Mocassin.UI.GUI.Base.Converter
{
    /// <summary>
    ///     Base class for typed <see cref="IValueConverter"/> implementations
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TParam"></typeparam>
    public abstract class ValueConverter<TIn, TOut, TParam> : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((TIn) value, (TParam) parameter, culture);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((TOut) value, (TParam) parameter, culture);
        }

        /// <summary>
        ///     Internal typed implementation of <see cref="Convert"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        protected abstract TOut Convert(TIn value, TParam parameter, CultureInfo culture);

        /// <summary>
        ///     Internal typed implementation of <see cref="ConvertBack"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        protected abstract TOut ConvertBack(TOut value, TParam parameter, CultureInfo culture);
    }
}