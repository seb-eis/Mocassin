using System;
using System.Reactive;
using System.Threading.Tasks;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Abstract base class for implementations of query managers
    /// </summary>
    internal abstract class ModelQueryManager : IModelQueryPort
    {
    }

    /// <summary>
    ///     Generic base class for model query manager implementations that support read only queries to reference data
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TPort"></typeparam>
    internal class ModelQueryManager<TData, TPort> : ModelQueryManager, IModelQueryPort<TPort>
        where TData : ModelData<TPort>
        where TPort : class, IModelDataPort
    {
        /// <summary>
        ///     Reader provider that provides safe data readers for the particle data
        /// </summary>
        protected DataReaderSource<TData, TPort> DataReaderSource { get; }

        /// <summary>
        ///     Creates a new query manager from the provided data object and data locker
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="lockSource"></param>
        public ModelQueryManager(TData baseData, AccessLockSource lockSource)
        {
            DataReaderSource = Basic.DataReaderSource.Create(baseData, baseData.AsReadOnly(), lockSource);
        }

        /// <inheritdoc />
        public virtual void Query(Action<TPort> action)
        {
            using (var reader = DataReaderSource.Create())
            {
                action(reader.Access);
            }
        }

        /// <inheritdoc />
        public virtual TResult Query<TResult>(Func<TPort, TResult> function)
        {
            using (var reader = DataReaderSource.Create())
            {
                return function(reader.Access);
            }
        }

        /// <inheritdoc />
        public Task<Unit> AsyncQuery(Action<TPort> action)
        {
            return Task.Run(() =>
            {
                Query(action);
                return Unit.Default;
            });
        }

        /// <inheritdoc />
        public Task<TResult> AsyncQuery<TResult>(Func<TPort, TResult> function)
        {
            return Task.Run(() => Query(function));
        }
    }

    /// <summary>
    ///     Generic base class for model query manager implementations that support read only queries to both reference data and
    ///     extended cached data
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataPort"></typeparam>
    /// <typeparam name="TDataCache"></typeparam>
    /// <typeparam name="TCachePort"></typeparam>
    internal class ModelQueryManager<TData, TDataPort, TDataCache, TCachePort> : ModelQueryManager<TData, TDataPort>,
        IModelQueryPort<TCachePort>
        where TData : ModelData<TDataPort>
        where TDataPort : class, IModelDataPort
        where TDataCache : ModelData<TCachePort>
        where TCachePort : class, IModelCachePort
    {
        /// <summary>
        ///     Reader provider that provides safe data readers for the particle data
        /// </summary>
        protected DataReaderSource<TDataCache, TCachePort> CacheReaderSource { get; }

        /// <summary>
        ///     Creates a new query manager from the provided data objects and data locker
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="cacheModel"></param>
        /// <param name="lockSource"></param>
        public ModelQueryManager(TData baseData, TDataCache cacheModel, AccessLockSource lockSource)
            : base(baseData, lockSource)
        {
            CacheReaderSource = Basic.DataReaderSource.Create(cacheModel, cacheModel.AsReadOnly(), lockSource);
        }

        /// <inheritdoc />
        public virtual void Query(Action<TCachePort> action)
        {
            using (var reader = CacheReaderSource.Create())
            {
                action(reader.Access);
            }
        }

        /// <inheritdoc />
        public virtual TResult Query<TResult>(Func<TCachePort, TResult> function)
        {
            using (var reader = CacheReaderSource.Create())
            {
                return function(reader.Access);
            }
        }

        /// <inheritdoc />
        public Task<Unit> AsyncQuery(Action<TCachePort> action)
        {
            return Task.Run(() =>
            {
                Query(action);
                return Unit.Default;
            });
        }

        /// <inheritdoc />
        public Task<TResult> AsyncQuery<TResult>(Func<TCachePort, TResult> function)
        {
            return Task.Run(() => Query(function));
        }
    }
}