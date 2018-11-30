using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Extensible adapter container for automatic management of exported data adapters and provision of affiliated
    ///     instances
    /// </summary>
    public class MmlAdapterContainer : IMmlAdapterSource
    {
        /// <summary>
        ///     The dictionary of defined adapter types with the collection of matching adapter instances
        /// </summary>
        private readonly IDictionary<Type, ICollection<object>> _adapterSets;

        /// <summary>
        /// The dictionary of defined injector types with the collection of matching injector instances
        /// </summary>
        private readonly IDictionary<Type, ICollection<object>> _injectorSets;

        /// <summary>
        /// The dictionary of defined extractor types with the collection of matching extractor instances
        /// </summary>
        private readonly IDictionary<Type, ICollection<object>> _extractorSets;

        /// <summary>
        ///     Creates new extensible adapter container
        /// </summary>
        protected MmlAdapterContainer()
        {
            _adapterSets = new Dictionary<Type, ICollection<object>>();
            _injectorSets = new Dictionary<Type, ICollection<object>>();
            _extractorSets = new Dictionary<Type, ICollection<object>>();
        }

        /// <summary>
        ///     Imports all adapters defined in the passed assembly, creates an instance for each and imports dependent adapter
        ///     onto marked properties
        /// </summary>
        /// <param name="assembly"></param>
        public void ImportAdapters(Assembly assembly)
        {
            AddAdapterInstances(assembly);
            InjectAdapterDependencies();
        }

        /// <inheritdoc />
        public IDataAdapter<TData, THandler> GetAdapter<TData, THandler>()
        {
            return GetAdapters<TData, THandler>().Single();
        }

        /// <inheritdoc />
        public IEnumerable<IDataAdapter<TData, THandler>> GetAdapters<TData, THandler>()
        {
            return GetAdapters<IDataAdapter<TData, THandler>>();
        }

        /// <inheritdoc />
        public TAdapter GetAdapter<TAdapter>()
        {
            return GetAdapters<TAdapter>().Single();
        }

        /// <inheritdoc />
        public IEnumerable<TAdapter> GetAdapters<TAdapter>()
        {
            return _adapterSets
                .Where(adapterSet => typeof(TAdapter).IsAssignableFrom(adapterSet.Key))
                .SelectMany(adapterSet => adapterSet.Value)
                .Cast<TAdapter>();
        }

        protected void AddAdapterInstances(Assembly assembly)
        {

        }

        protected void InjectAdapterDependencies()
        {

        }
    }
}