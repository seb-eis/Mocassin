using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mocassin.Framework.Reflection;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Abstract base class for reflective data extractors that automatically build their extraction pipeline trough
    ///     reflection on construction
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public abstract class ReflectiveDataExtractor<THandler, TData> : IDataExtractor<THandler, TData>
        where TData : new()
    {
        /// <summary>
        ///     The collection of operations that is performed during extraction
        /// </summary>
        private readonly ICollection<Action<TData, THandler>> _extractionOperations;

        /// <summary>
        /// Creates new data extractor and automatically build the extraction pipeline from marked methods 
        /// </summary>
        protected ReflectiveDataExtractor()
        {
            _extractionOperations = GetExtractionOperations(this);
        }

        /// <inheritdoc />
        public TData ExtractData(THandler handler)
        {
            var data = new TData();
            foreach (var action in _extractionOperations)
                action(data, handler);

            return data;
        }

        /// <summary>
        ///     Static method to search an object for marked extraction operations and return the collection of found delegates
        ///     bound to the passed instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static ICollection<Action<TData, THandler>> GetExtractionOperations(object instance)
        {
            if (instance == null) 
                throw new ArgumentNullException(nameof(instance));

            int SortComparison(Delegate lhs, Delegate rhs)
            {
                var lhsOrder = lhs.GetMethodInfo().GetCustomAttribute<ExtractionMethodAttribute>().Order;
                var rhsOrder = rhs.GetMethodInfo().GetCustomAttribute<ExtractionMethodAttribute>().Order;
                return lhsOrder.CompareTo(rhsOrder);
            }

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var creator = new DelegateCreator();
            var delegates = creator.CreateWhere(instance, info => info.GetCustomAttribute<ExtractionMethodAttribute>() != null, flags)
                .Cast<Action<TData, THandler>>()
                .ToList();

            delegates.Sort(SortComparison);
            return delegates;
        }
    }
}