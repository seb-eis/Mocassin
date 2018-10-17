using System;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Abstract base class for all model data reader providers that create safe data readers for read only model data
    ///     access
    /// </summary>
    public abstract class DataReaderSource
    {
        /// <summary>
        ///     The data locker that tries to get a valid lock on a data object for its internally specified attempt time
        /// </summary>
        protected AccessLockSource LockSource { get; }

        /// <summary>
        ///     Creates new model data reader provider base for read only access with the provided data locker
        /// </summary>
        /// <param name="lockSource"></param>
        protected DataReaderSource(AccessLockSource lockSource)
        {
            LockSource = lockSource ?? throw new ArgumentNullException(nameof(lockSource));
        }

        /// <summary>
        ///     Factory to create new model data readers for the provided data object and
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TPort"></typeparam>
        /// <param name="data"></param>
        /// <param name="dataPort"></param>
        /// <param name="lockSource"></param>
        /// <returns></returns>
        public static DataReaderSource<TData, TPort> Create<TData, TPort>(TData data, TPort dataPort, AccessLockSource lockSource)
            where TData : ModelData<TPort>
            where TPort : class, IModelDataPort
        {
            return new DataReaderSource<TData, TPort>(data, lockSource);
        }
    }

    /// <summary>
    ///     Generic provider for data readers for model data objects completely hiding the write access to the data object
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TPort"></typeparam>
    public class DataReaderSource<TData, TPort> : DataReaderSource, IDataReaderSource<TPort>
        where TData : ModelData<TPort>
        where TPort : class, IModelDataPort
    {
        /// <summary>
        ///     Data object used for reader provision
        /// </summary>
        private TData DataObject { get; }

        /// <summary>
        ///     Generates new data reader provider for a data object
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="lockSource"></param>
        public DataReaderSource(TData dataObject, AccessLockSource lockSource)
            : base(lockSource)
        {
            DataObject = dataObject ?? throw new ArgumentNullException(nameof(dataObject));
        }

        /// <summary>
        ///     Creates a new model data reader for the internal data object
        /// </summary>
        /// <returns></returns>
        public DataReader<TData, TPort> Create()
        {
            return new DataReader<TData, TPort>(DataObject, LockSource);
        }

        /// <inheritdoc />
        public IDataReader<TPort> CreateInterface()
        {
            return Create();
        }
    }
}