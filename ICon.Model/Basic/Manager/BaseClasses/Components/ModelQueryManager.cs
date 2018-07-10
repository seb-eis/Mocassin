using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base class for implementations of query managers
    /// </summary>
    internal abstract class ModelQueryManager : IModelQueryPort
    {
    }

    /// <summary>
    /// Generic base class for model query manager implementations that support read only queries to refernce data
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataPort"></typeparam>
    internal class ModelQueryManager<TData, TDataPort> : ModelQueryManager, IModelQueryPort<TDataPort>
        where TData : ModelData<TDataPort>
        where TDataPort : class, IModelDataPort
    {
        /// <summary>
        /// Reader provider that provides safe data readers for the particle data
        /// </summary>
        protected DataReadProvider<TData, TDataPort> DataReaderProvider { get; }

        /// <summary>
        /// Creates a new query manager from the provided data object and data locker
        /// </summary>
        /// <param name="baseData"></param>
        public ModelQueryManager(TData baseData, DataAccessLocker dataLocker)
        {
            DataReaderProvider = Basic.DataReadProvider.Create(baseData, baseData.AsReadOnly(), dataLocker);
        }

        /// <summary>
        /// Performs a data query that returns no value using the read only data port, the manager data is locked for writing until query completion
        /// </summary>
        /// <param name="query"></param>
        public virtual void Query(Action<TDataPort> query)
        {
            using (var reader = DataReaderProvider.Create())
            {
                query(reader.Access);
            }
        }

        /// <summary>
        /// Performs a data query that returns a value using the read only data port, the manager data is lcoked for writing until query completion
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual TResult Query<TResult>(Func<TDataPort, TResult> query)
        {
            using (var reader = DataReaderProvider.Create())
            {
                return query(reader.Access);
            }
        }

        /// <summary>
        /// Performs an async data query that returns no value using the read only data port, the manager data is locked for writing until query completion
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<Unit> AsyncQuery(Action<TDataPort> query)
        {
            return Task.Run(() => { Query(query); return Unit.Default; });
        }

        /// <summary>
        /// Performs an async data query that returns a value using the read only data port, the manager data is lcoked for writing until query completion
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<TResult> AsyncQuery<TResult>(Func<TDataPort, TResult> query)
        {
            return Task.Run(() => Query(query));
        }
    }

    /// <summary>
    /// Generic base class for model query manager implementations that support read only queries to both refernce data and extended cached data
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataPort"></typeparam>
    internal class ModelQueryManager<TData, TDataPort, TDataCache, TCachePort> : ModelQueryManager<TData, TDataPort>, IModelQueryPort<TCachePort>
        where TData : ModelData<TDataPort>
        where TDataPort : class, IModelDataPort
        where TDataCache : ModelData<TCachePort>
        where TCachePort : class, IModelCachePort
    {
        /// <summary>
        /// Reader provider that provides safe data readers for the particle data
        /// </summary>
        protected DataReadProvider<TDataCache, TCachePort> CacheReaderProvider { get; }

        /// <summary>
        /// Creates a new query manager from the provided data objects and data locker
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="cacheData"></param>
        /// <param name="dataLocker"></param>
        public ModelQueryManager(TData baseData, TDataCache cacheData, DataAccessLocker dataLocker) : base(baseData, dataLocker)
        {
            CacheReaderProvider = DataReadProvider.Create(cacheData, cacheData.AsReadOnly(), dataLocker);
        }

        /// <summary>
        /// Performs a data query that returns no value using the read only data port of the extended cached data, the manager data is locked for writing until query completion
        /// </summary>
        /// <param name="query"></param>
        public virtual void Query(Action<TCachePort> query)
        {
            using (var reader = CacheReaderProvider.Create())
            {
                query(reader.Access);
            }
        }

        /// <summary>
        /// Performs a data query that returns a value using the read only data port of the extended cached data, the manager data is lcoked for writing until query completion
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual TResult Query<TResult>(Func<TCachePort, TResult> query)
        {
            using (var reader = CacheReaderProvider.Create())
            {
                return query(reader.Access);
            }
        }

        /// <summary>
        /// Performs an async data query that returns no value using the read only data port of the extended cached data, the manager data is locked for writing until query completion
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<Unit> AsyncQuery(Action<TCachePort> query)
        {
            return Task.Run(() => { Query(query); return Unit.Default; });
        }

        /// <summary>
        /// Performs an async data query that returns a value using the read only data port of the extended cached data, the manager data is lcoked for writing until query completion
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<TResult> AsyncQuery<TResult>(Func<TCachePort, TResult> query)
        {
            return Task.Run(() => Query(query));
        }
    }
}
