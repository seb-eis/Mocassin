using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mocassin.Framework.Operations;
using Mocassin.Framework.Reflection;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Abstract base class for reflective data adapters that supply a set of independent injection and extraction
    ///     operations that can be looked up through attribute tags
    /// </summary>
    public abstract class ReflectiveDataAdapter<TData, THandler> : IDataAdapter<TData, THandler>
        where TData : new()
    {
        /// <summary>
        ///     Defines the collection of extraction operations to be performed
        /// </summary>
        private readonly ICollection<Action<TData, THandler>> _extractionOperations;

        /// <summary>
        ///     Defines the collection of injection operations to be performed
        /// </summary>
        private readonly ICollection<Action<TData, THandler, IOperationReport>> _injectionOperations;

        /// <summary>
        /// Creates a new reflective data adapter with automatically created operation sets
        /// </summary>
        protected ReflectiveDataAdapter()
        {
            _extractionOperations = ReflectiveDataExtractor<THandler, TData>.GetExtractionOperations(this);
            _injectionOperations = ReflectiveDataInjector<TData, THandler>.GetInjectionOperations(this);
        }

        /// <inheritdoc />
        public TData ExtractData(THandler handler)
        {
            var data = new TData();
            foreach (var action in _extractionOperations)
                action(data, handler);

            return data;
        }

        /// <inheritdoc />
        public IOperationReport InjectData(TData data, THandler handler)
        {
            var report = new OperationReport();
            foreach (var action in _injectionOperations)
                action(data, handler, report);

            return report;
        }
    }
}