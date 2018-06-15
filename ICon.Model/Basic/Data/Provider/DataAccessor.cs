using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive;
using System.Threading.Tasks;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Genric disposbale model data accessor that fully locks a data object for other access attempts until disposed (Mainly used for write operations)
    /// </summary>
    public class DataAccessor<TData> : IDataAccessor<TData> where TData : ModelData
    {
        /// <summary>
        /// The data object that should be modified
        /// </summary>
        private TData Data { get; set; }

        /// <summary>
        /// The disposable lock on the data object
        /// </summary>
        private IDisposable Lock { get; set; }

        /// <summary>
        /// Cretaes new data writer for a data object, throw exception on locking timeout
        /// </summary>
        /// <param name="data"></param>
        public DataAccessor(TData data, DataAccessLocker locker)
        {
            if (locker == null)
            {
                throw new ArgumentNullException(nameof(locker));
            }
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Lock = locker.TryGetFullAccessLock(data);
        }

        /// <summary>
        /// Performs an action on the wrapped data object
        /// </summary>
        /// <param name="action"></param>
        public void Query(Action<TData> action)
        {
            action(Data);
        }

        /// <summary>
        /// Performs an action with return value on the wrapped data object
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        public TResult Query<TResult>(Func<TData, TResult> function)
        {
            return function(Data);
        }

        /// <summary>
        /// Ensures that a potentially remaining data lock is released and nulls the refernce to the data object preventing further access operations
        /// </summary>
        public void Dispose()
        {
            Data = null;
            Lock?.Dispose();
        }

        /// <summary>
        /// Tries to create a reader interface for the data object that matches the specified port type (Returns null if not possible)
        /// </summary>
        /// <typeparam name="TDataPort"></typeparam>
        /// <returns></returns>
        public IDataReader<TDataPort> AsReader<TDataPort>() where TDataPort : class, IModelDataPort
        {
            return new DataAccessorReadOnlyWrapper<TData, TDataPort>(this);
        }

        /// <summary>
        /// Creates a read only wrapper for the accessor matching the data port of the specified reader provider
        /// </summary>
        /// <typeparam name="TDataPort"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public IDataReader<TDataPort> AsReader<TDataPort>(IDataReaderProvider<TDataPort> provider) where TDataPort : class, IModelDataPort
        {
            return AsReader<TDataPort>();
        }
    }

    /// <summary>
    /// Wrapper for data accessors that restricts to port based reading access
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataPort"></typeparam>
    public class DataAccessorReadOnlyWrapper<TData, TDataPort> : IDataReader<TDataPort> where TData : ModelData where TDataPort : class, IModelDataPort
    {
        /// <summary>
        /// Access the data port
        /// </summary>
        public TDataPort Access { get; set; }

        /// <summary>
        /// The rwapped accessor object
        /// </summary>
        protected DataAccessor<TData> Accessor { get; set; }

        /// <summary>
        /// Creates new read only wrapped data accessor
        /// </summary>
        /// <param name="accessor"></param>
        public DataAccessorReadOnlyWrapper(DataAccessor<TData> accessor)
        {
            TDataPort dataPort = accessor.Query(data => data.GetModelDataPort() as TDataPort);
            if (dataPort == null)
            {
                throw new ArgumentException("The data object of the accessor does not support the specififed data port", nameof(accessor));
            }
            Access = accessor.Query(data => data.GetModelDataPort() as TDataPort);
            Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        /// <summary>
        /// Disposes the wrapper by nulling the accessor and data port (Does not dispose of parent)
        /// </summary>
        public void Dispose()
        {
            Accessor = null;
            Access = null;
        }
    }
}
