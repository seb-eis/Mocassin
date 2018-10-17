using System;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Base class for all model data writer providers that offers data writers fro model data
    /// </summary>
    public abstract class DataAccessorSource
    {
        /// <summary>
        ///     The data locker that tries to get a valid lock on a data object for its internally specified attempt time
        /// </summary>
        protected AccessLockSource LockSource { get; }

        /// <summary>
        ///     Creates new data model writer provider with the specified data locker
        /// </summary>
        /// <param name="lockSource"></param>
        protected DataAccessorSource(AccessLockSource lockSource)
        {
            LockSource = lockSource ?? throw new ArgumentNullException(nameof(lockSource));
        }

        /// <summary>
        ///     Factory to create new model data writer providers
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <param name="lockSource"></param>
        /// <returns></returns>
        public static DataAccessSource<TData> Create<TData>(TData data, AccessLockSource lockSource)
            where TData : ModelData
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            return new DataAccessSource<TData>(data, lockSource);
        }
    }

    /// <summary>
    ///     Generic provider for data accessors for model data objects
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class DataAccessSource<TData> : DataAccessorSource, IDataAccessorSource<TData>
        where TData : ModelData
    {
        /// <summary>
        ///     Data object used for writing provision
        /// </summary>
        private TData DataObject { get; }

        /// <summary>
        ///     Generates new data writer provider for a data object
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="lockSource"></param>
        public DataAccessSource(TData dataObject, AccessLockSource lockSource)
            : base(lockSource)
        {
            DataObject = dataObject ?? throw new ArgumentNullException(nameof(dataObject));
        }

        /// <summary>
        ///     Creates a new model data accessor for the internal data object
        /// </summary>
        /// <returns></returns>
        public DataAccessor<TData> Create()
        {
            return new DataAccessor<TData>(DataObject, LockSource);
        }

        /// <inheritdoc />
        public IDataAccessor<TData> CreateInterface()
        {
            return Create();
        }
    }
}