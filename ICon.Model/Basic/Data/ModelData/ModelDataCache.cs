using System;
using System.Collections.Generic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Abstract base class for all implementations of extended data containers that supply calculated data on a
    ///     'on-demand' basis and cache the results for faster access through object providers
    /// </summary>
    /// <typeparam name="TPort"></typeparam>
    public abstract class ModelDataCache<TPort> : ModelData<TPort> where TPort : IModelCachePort
    {
        /// <summary>
        ///     The project services instance that is used to create read only wrappers for the cache
        /// </summary>
        protected IProjectServices ProjectServices { get; }

        /// <summary>
        ///     The subscription the the relevant update events
        /// </summary>
        protected IDisposable[] EventSubscriptions { get; set; }

        /// <summary>
        ///     The cache port (Stores the cache port implementation after first creation)
        /// </summary>
        protected TPort CachePort { get; set; }

        /// <summary>
        ///     List that contains all the cached data objects
        /// </summary>
        public List<ICachedObjectSource> DataCache { get; set; }

        /// <summary>
        ///     Flag that is set after initialization. Prohibits another initialization process
        /// </summary>
        public bool IsInitialized { get; set; }

        /// <summary>
        ///     Creates new extended data object with empty cache list and registers to the provided event port events to register
        ///     notifications about expired cached data
        /// </summary>
        protected ModelDataCache(IModelEventPort eventPort, IProjectServices projectServices)
        {
            if (eventPort == null) 
                throw new ArgumentNullException(nameof(eventPort));

            SubscribeToEventPort(eventPort);
            DataCache = new List<ICachedObjectSource>();
            ProjectServices = projectServices ?? throw new ArgumentNullException(nameof(projectServices));
        }

        /// <summary>
        ///     Initialize the cache if not initialized before and set the initialization flag
        /// </summary>
        /// <param name="dataObject"></param>
        public void Initialize(IEnumerable<ICachedObjectSource> dataObject)
        {
            if (IsInitialized) 
                return;

            foreach (var item in dataObject)
                DataCache.Add(item);

            IsInitialized = true;
        }

        /// <summary>
        ///     Searches the cache for an entry that matches the creation method
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="creationMethod"></param>
        /// <returns></returns>
        public ICachedObjectSource FindCacheEntry<T1>(Func<T1> creationMethod)
        {
            return DataCache.Find(entry => entry.FactoryDelegate.Equals(creationMethod)) ??
                   throw new InvalidOperationException($"Type not cached {typeof(T1)}");
        }

        /// <summary>
        ///     Clear all cached data by resetting it to their default values
        /// </summary>
        public virtual void ClearAll()
        {
            foreach (var data in DataCache)
                data?.Clear();
        }

        /// <inheritdoc />
        public override void ResetToDefault()
        {
            ClearAll();
        }

        /// <summary>
        ///     Marks all cached data as deprecated
        /// </summary>
        protected void MarkAllAsObsolete()
        {
            foreach (var data in DataCache) 
                data?.MarkAsDeprecated();
        }

        /// <summary>
        ///     Registers delegates to manager reset, manager disconnect and extended data expired events of the provided event
        ///     port
        /// </summary>
        /// <param name="eventPort"></param>
        protected void SubscribeToEventPort(IModelEventPort eventPort)
        {
            EventSubscriptions = new[]
            {
                eventPort.WhenExtendedDataExpired.Subscribe(x => MarkAllAsObsolete()),
                eventPort.WhenManagerReset.Subscribe(x => MarkAllAsObsolete()),
                eventPort.WhenManagerDisconnects.Subscribe(x => DisposeSubscriptions())
            };
        }

        /// <summary>
        ///     Disposes all subscriptions that are not already disposed
        /// </summary>
        protected void DisposeSubscriptions()
        {
            foreach (var item in EventSubscriptions) 
                item?.Dispose();
        }
    }
}