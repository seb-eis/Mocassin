using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Base class for all model data writer providers that offers data writers fro model data
    /// </summary>
    public abstract class DataAccessProvider
    {
        /// <summary>
        /// The data locker that tries to get a valid lock on a data object for its internally specified attempt time
        /// </summary>
        protected DataAccessLocker DataLocker { get; }

        /// <summary>
        /// Creates new data model writer provider with the specified data locker
        /// </summary>
        /// <param name="dataLocker"></param>
        public DataAccessProvider(DataAccessLocker dataLocker)
        {
            DataLocker = dataLocker ?? throw new ArgumentNullException(nameof(dataLocker));
        }

        /// <summary>
        /// Factory to create new model data writer providers
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <param name="dataLocker"></param>
        /// <returns></returns>
        public static DataAccessProvider<TData> Create<TData>(TData data, DataAccessLocker dataLocker) where TData : ModelData
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            return new DataAccessProvider<TData>(data, dataLocker);
        }
    }

    /// <summary>
    /// Generic provider for data accessors for model data objects
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class DataAccessProvider<TData> : DataAccessProvider, IDataAccessorProvider<TData> where TData : ModelData
    {
        /// <summary>
        /// Data object used for writing provision
        /// </summary>
        private TData DataObject { get; }

        /// <summary>
        /// Generates new data writer provider for a data object
        /// </summary>
        /// <param name="dataObject"></param>
        public DataAccessProvider(TData dataObject, DataAccessLocker dataLocker) : base(dataLocker)
        {
            DataObject = dataObject ?? throw new ArgumentNullException(nameof(dataObject));
        }

        /// <summary>
        /// Creates a new model data accessor for the internal data object
        /// </summary>
        /// <returns></returns>
        public DataAccessor<TData> Create()
        {
            return new DataAccessor<TData>(DataObject, DataLocker);
        }

        /// <summary>
        /// Creates new write interface to the stored data object
        /// </summary>
        /// <returns></returns>
        public IDataAccessor<TData> CreateInterface()
        {
            return Create();
        }
    }
}
