using System;
using System.Reactive.Disposables;

namespace Mocassin.Model.Basic
{
    /// <inheritdoc />
    public class DataAccessor<TData> : IDataAccessor<TData> where TData : ModelData
    {
        /// <summary>
        ///     The data object that should be modified
        /// </summary>
        private TData Data { get; set; }

        /// <summary>
        ///     The disposable lock on the data object
        /// </summary>
        private IDisposable Lock { get; }

        /// <summary>
        ///     Creates new data writer for a data object, throw exception on locking timeout
        /// </summary>
        /// <param name="data"></param>
        /// <param name="lockSource"></param>
        public DataAccessor(TData data, AccessLockSource lockSource)
        {
            if (lockSource == null)
                throw new ArgumentNullException(nameof(lockSource));

            Data = data ?? throw new ArgumentNullException(nameof(data));
            Lock = lockSource.TryGetFullAccess(data);
        }

        /// <summary>
        ///     Creates new data writer for a data object that is not require an access lock
        /// </summary>
        /// <param name="data"></param>
        public DataAccessor(TData data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Lock = Disposable.Empty;
        }

        /// <inheritdoc />
        public void Query(Action<TData> action)
        {
            action(Data);
        }

        /// <inheritdoc />
        public TResult Query<TResult>(Func<TData, TResult> function)
        {
            return function(Data);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Data = null;
            Lock?.Dispose();
        }

        /// <inheritdoc />
        public IDataReader<TPort> AsReader<TPort>()
            where TPort : class, IModelDataPort
        {
            return new ReadOnlyDataAccessorAdapter<TData, TPort>(this);
        }

        /// <inheritdoc />
        public IDataReader<TPort> AsReader<TPort>(IDataReaderSource<TPort> source)
            where TPort : class, IModelDataPort
        {
            return AsReader<TPort>();
        }
    }

    /// <summary>
    ///     Adapter class for data accessors that restricts to port based reading access
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TPort"></typeparam>
    public class ReadOnlyDataAccessorAdapter<TData, TPort> : IDataReader<TPort>
        where TData : ModelData
        where TPort : class, IModelDataPort
    {
        /// <inheritdoc />
        public TPort Access { get; set; }

        /// <summary>
        ///     The wrapped accessor object
        /// </summary>
        protected DataAccessor<TData> Accessor { get; set; }

        /// <summary>
        ///     Creates new read only wrapped data accessor
        /// </summary>
        /// <param name="accessor"></param>
        public ReadOnlyDataAccessorAdapter(DataAccessor<TData> accessor)
        {
            if (accessor == null)
                throw new ArgumentNullException(nameof(accessor));

            var dataPort = accessor.Query(data => data.GetModelDataPort() as TPort);
            if (dataPort == null)
                throw new ArgumentException("The data object of the accessor does not support the specified data port", nameof(accessor));

            Access = accessor.Query(data => data.GetModelDataPort() as TPort);
            Accessor = accessor;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Accessor = null;
            Access = null;
        }
    }
}