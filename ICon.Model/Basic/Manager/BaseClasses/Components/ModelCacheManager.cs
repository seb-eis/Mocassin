using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Abstract base class for all manager implementations that handle 'on demand' extended model data and cache the
    ///     results until deprecation
    /// </summary>
    internal abstract class ModelCacheManager : IModelCachePort
    {
        /// <inheritdoc />
        public abstract object GetDataCopy();

        /// <inheritdoc />
        public abstract void ClearCachedData();

        /// <inheritdoc />
        public abstract string JsonSerializeData();

        /// <inheritdoc />
        public abstract void WriteDataContract(Stream stream, DataContractSerializerSettings settings);
    }

    /// <summary>
    ///     Generic abstract base class for all model cache managers that support a specific extended data type through a
    ///     specified cache data port
    /// </summary>
    /// <typeparam name="TCache"></typeparam>
    /// <typeparam name="TPort"></typeparam>
    internal abstract class ModelCacheManager<TCache, TPort> : ModelCacheManager
        where TPort : IModelCachePort
        where TCache : ModelDataCache<TPort>
    {
        /// <summary>
        ///     The project services instance to access shared services and other managers
        /// </summary>
        protected IModelProject ModelProject { get; set; }

        /// <summary>
        ///     The extended data object that supports a caching system
        /// </summary>
        protected TCache Cache { get; set; }

        /// <summary>
        ///     Create a new model cache manager for the provided model data using the provided project services
        /// </summary>
        /// <param name="modelCache"></param>
        /// <param name="modelProject"></param>
        protected ModelCacheManager(TCache modelCache, IModelProject modelProject)
        {
            Cache = modelCache ?? throw new ArgumentNullException(nameof(modelCache));
            ModelProject = modelProject ?? throw new ArgumentNullException(nameof(modelProject));
            InitializeIfNotInitialized();
        }

        /// <summary>
        ///     Get the data that is created by the specified creator delegate from the cache and returns the cast value
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="creatorMethod"></param>
        /// <returns></returns>
        protected TResult GetResultFromCache<TResult>(Func<TResult> creatorMethod)
        {
            return (TResult) Cache.FindCacheEntry(creatorMethod).GetValue();
        }

        /// <inheritdoc />
        public override void ClearCachedData()
        {
            foreach (var data in Cache.DataCache)
                data?.Clear();
        }

        /// <summary>
        ///     Clears only deprecated cached data
        /// </summary>
        public virtual void ClearDeprecatedCachedData()
        {
            foreach (var data in Cache.DataCache)
                data?.ClearIfDeprecated();
        }

        /// <inheritdoc />
        public override object GetDataCopy()
        {
            throw new NotImplementedException("Data copy of cached data currently not supported");
        }

        /// <inheritdoc />
        public override string JsonSerializeData()
        {
            throw new NotImplementedException("Json serialization for cached data currently not supported");
        }

        /// <inheritdoc />
        public override void WriteDataContract(Stream stream, DataContractSerializerSettings settings)
        {
            throw new NotImplementedException("Data contract serialization for cached data currently not supported");
        }

        /// <summary>
        ///     Initializes the cache, this binds the first created instance of the cache manager to the creation methods
        /// </summary>
        protected void InitializeIfNotInitialized()
        {
            if (Cache.IsInitialized)
                return;

            Cache.Initialize(FindAndMakeCacheEntries());
        }

        /// <summary>
        ///     Searches the manager for all <see cref="CacheMethodResultAttribute" /> marked methods and creates the sequence of
        ///     <see cref="ICachedObjectSource"/> provider interfaces
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<ICachedObjectSource> FindAndMakeCacheEntries()
        {
            foreach (var method in GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (!(method.GetCustomAttribute<CacheMethodResultAttribute>() is CacheMethodResultAttribute attribute))
                    continue;

                var delegateType = typeof(Func<>).MakeGenericType(method.ReturnType);
                var wrapperType = attribute.GenericDataWrapperType.MakeGenericType(method.ReturnType);

                ICachedObjectSource cachedData;
                try
                {
                    cachedData = (ICachedObjectSource) Activator.CreateInstance(wrapperType, method.CreateDelegate(delegateType, this));
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("Instance cached object provider failed", e);
                }

                yield return cachedData;
            }
        }
    }
}