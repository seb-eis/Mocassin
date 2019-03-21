using System;
using System.Collections.Generic;
using System.Globalization;

namespace Mocassin.UI.GUI.Base.Converter
{
    /// <summary>
    ///     Unique <see cref="ValueConverter{TIn,TOut,TParam}" /> that supports unique value conversion by
    ///     <see cref="IDictionary{TKey,TValue}" /> lookup
    /// </summary>
    public class UniqueLookupConverter<TIn, TOut, TParam> : ValueConverter<TIn, TOut, TParam>
    {
        /// <summary>
        ///     Get the <see cref="IDictionary{TKey,TValue}" /> that is used for value conversion
        /// </summary>
        public IDictionary<TIn, TOut> ConversionDictionary { get; }

        /// <inheritdoc />
        public UniqueLookupConverter()
        {
            ConversionDictionary = new Dictionary<TIn, TOut>();
        }

        /// <summary>
        ///     Indexer to access the <see cref="TOut"/> lookup <see cref="IDictionary{TKey,TValue}"/>
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual TOut this[TIn param] => ConversionDictionary[param];

        /// <summary>
        ///     Creates new <see cref="UniqueLookupConverter{TIn,TOut,TParam}" /> that used the provided conversion
        ///     <see cref="IDictionary{TKey,TValue}" />
        /// </summary>
        /// <param name="conversionDictionary"></param>
        public UniqueLookupConverter(IDictionary<TIn, TOut> conversionDictionary)
        {
            ConversionDictionary = conversionDictionary ?? throw new ArgumentNullException(nameof(conversionDictionary));
        }

        /// <inheritdoc />
        public override TOut Convert(TIn value, TParam parameter, CultureInfo culture)
        {
            return ConversionDictionary[value];
        }
    }
}