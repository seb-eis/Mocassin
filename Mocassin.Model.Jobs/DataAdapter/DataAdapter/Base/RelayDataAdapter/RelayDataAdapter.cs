using System;
using Mocassin.Framework.Operations;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Generic class for a relay data adapter that bundles extractor and injector components to provide
    ///     the data adapter functionality
    /// </summary>
    public class RelayDataAdapter<TData, THandler> : IDataAdapter<TData, THandler>
    {
        /// <summary>
        ///     Backing field for the extractor
        /// </summary>
        private IDataExtractor<THandler, TData> _extractor;

        /// <summary>
        ///     Backing field for the injector
        /// </summary>
        private IDataInjector<TData, THandler> _injector;

        /// <summary>
        ///     Get or set the data extractor that the relay uses
        /// </summary>
        public virtual IDataExtractor<THandler, TData> Extractor
        {
            get => _extractor;
            protected set => _extractor = value;
        }

        /// <summary>
        ///     Get or set the data injector that the relay uses
        /// </summary>
        public virtual IDataInjector<TData, THandler> Injector
        {
            get => _injector;
            protected set => _injector = value;
        }

        /// <summary>
        ///     Creates a new relay data adapter from extractor and injector interfaces
        /// </summary>
        /// <param name="extractor"></param>
        /// <param name="injector"></param>
        public RelayDataAdapter(IDataExtractor<THandler, TData> extractor, IDataInjector<TData, THandler> injector)
        {
            _extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
            _injector = injector ?? throw new ArgumentNullException(nameof(injector));
        }

        /// <summary>
        ///     Creates new empty relay data adepter
        /// </summary>
        public RelayDataAdapter()
        {
        }

        /// <inheritdoc />
        public IOperationReport InjectData(TData data, THandler handler)
        {
            return Injector.InjectData(data, handler);
        }

        /// <inheritdoc />
        public TData ExtractData(THandler handler)
        {
            return Extractor.ExtractData(handler);
        }
    }
}