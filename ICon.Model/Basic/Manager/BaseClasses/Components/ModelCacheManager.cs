using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;

using ICon.Model.ProjectServices;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base class for all manager implementations that handle 'on demand' extended model data and cache the results until deprecation
    /// </summary>
    internal abstract class ModelCacheManager : IModelCachePort
    {
        /// <summary>
        /// Get a deep data copy
        /// </summary>
        /// <returns></returns>
        public abstract object GetDataCopy();

        /// <summary>
        /// Json serialize the cached data
        /// </summary>
        /// <returns></returns>
        public abstract string JsonSerializeData();

        /// <summary>
        /// Write the data contract to a stream using the provided settings
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="settings"></param>
        public abstract void WriteDataContract(Stream stream, DataContractSerializerSettings settings);
    }

    /// <summary>
    /// Generic abstract base class for all model cache managers that support a specific extended data type through a specified cache data port
    /// </summary>
    /// <typeparam name="TDataCache"></typeparam>
    /// <typeparam name="TQueryPort"></typeparam>
    /// <typeparam name="TCachePort"></typeparam>
    internal abstract class ModelCacheManager<TDataCache, TCachePort> : ModelCacheManager
        where TCachePort : IModelCachePort
        where TDataCache : DynamicModelDataCache<TCachePort>
    {
        /// <summary>
        /// The project services instance to access shared services and other managers
        /// </summary>
        protected IProjectServices ProjectServices { get; set; }

        /// <summary>
        /// The extended data object that supports a caching system
        /// </summary>
        protected TDataCache Cache { get; set; }

        /// <summary>
        /// Create a new model cache manager for the provided model data using the provided project services
        /// </summary>
        /// <param name="dataCache"></param>
        /// <param name="dataManager"></param>
        /// <param name="projectServices"></param>
        protected ModelCacheManager(TDataCache dataCache, IProjectServices projectServices) : base()
        {
            Cache = dataCache ?? throw new ArgumentNullException(nameof(dataCache));
            ProjectServices = projectServices ?? throw new ArgumentNullException(nameof(projectServices));
            InitializeIfNotInitialized();
        }

        /// <summary>
        /// Get the data that is created by the specififed creator delegate from the cache and returns the cast value
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="creatorMethod"></param>
        /// <returns></returns>
        protected TResult AccessCacheableDataEntry<TResult>(Func<TResult> creatorMethod)
        {
            return (TResult)Cache.FindCacheEntry(creatorMethod).GetValue();
        }

        /// <summary>
        /// Clears all cached data objects causing them to be recalculated on the next data access
        /// </summary>
        public virtual void ClearCachedData()
        {
            foreach (ICachedData data in Cache.DataCache)
            {
                data?.Clear();
            }
        }

        /// <summary>
        /// Clears only deprecated cached data
        /// </summary>
        public virtual void ClearDeprecatedCachedData()
        {
            foreach (ICachedData data in Cache.DataCache)
            {
                data?.ClearIfDeprecated();
            }
        }

        /// <summary>
        /// Get a copy of the internal data object by json serialization (Not implemented)
        /// </summary>
        /// <returns></returns>
        public override object GetDataCopy()
        {
            throw new NotImplementedException("Data copy of cached data currently not supported");
        }

        /// <summary>
        /// Json serialize the data object (Not implemented)
        /// </summary>
        /// <returns></returns>
        public override string JsonSerializeData()
        {
            throw new NotImplementedException("Json serialization for cached data currently not supported");
        }

        /// <summary>
        /// Use the data write to a stream of data contract serializer with the specified settings (Not implemented)
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="settings"></param>
        public override void WriteDataContract(Stream stream, DataContractSerializerSettings settings)
        {
            throw new NotImplementedException("Data contract serialization for cached data currently not supported");
        }

        /// <summary>
        /// Initializes the cache, this binds the first created instance of the cahe manager to the creation methods
        /// </summary>
        protected void InitializeIfNotInitialized()
        {
            if (Cache.IsInitialized)
            {
                return;
            }
            Cache.Initialize(FindAndMakeCacheEntries());
        }

        /// <summary>
        /// Searches the manager for all marked methods and creates the sequence of cahe objects
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<ICachedData> FindAndMakeCacheEntries()
        {
            foreach (var method in GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (method.GetCustomAttribute(typeof(CacheableMethodAttribute)) is CacheableMethodAttribute attribute)
                {
                    var delegateType = typeof(Func<>).MakeGenericType(method.ReturnType);
                    var wrapperType = attribute.GenericDataWrapperType.MakeGenericType(method.ReturnType);

                    var cachedData = (ICachedData)Activator.CreateInstance(wrapperType, method.CreateDelegate(delegateType, this));
                    if (cachedData == null)
                    {
                        throw new InvalidOperationException("Instance creation of cached data interface by attribute properties failed");
                    }
                    yield return cachedData;
                }
            }
        }
    }
}
