using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base class for all model data reader providers that create safe data readers for read only model data access
    /// </summary>
    public abstract class DataReadProvider
    {
        /// <summary>
        /// The data locker that tries to get a valid lock on a data object for its internally specified attempt time
        /// </summary>
        protected DataAccessLocker DataLocker { get; }

        /// <summary>
        /// Creates new model data reader provider base for read only access with the provided data locker
        /// </summary>
        /// <param name="dataLocker"></param>
        protected DataReadProvider(DataAccessLocker dataLocker)
        {
            DataLocker = dataLocker ?? throw new ArgumentNullException(nameof(dataLocker));
        }

        /// <summary>
        /// Factory to create new model data readers for the provided data object and
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TDataPort"></typeparam>
        /// <param name="data"></param>
        /// <param name="dataPort"></param>
        /// <returns></returns>
        public static DataReadProvider<TData, TDataPort> Create<TData, TDataPort>(TData data, TDataPort dataPort, DataAccessLocker dataLocker)
            where TData : ModelData<TDataPort> where TDataPort : class, IModelDataPort
        {
            return new DataReadProvider<TData, TDataPort>(data, dataLocker);
        }
    }

    /// <summary>
    /// Generic provider for data readers for model data objects completly hiding the write access to the data object
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataPort"></typeparam>
    public class DataReadProvider<TData, TDataPort> : DataReadProvider, IDataReaderProvider<TDataPort>
        where TData : ModelData<TDataPort> where TDataPort : class, IModelDataPort
    {
        /// <summary>
        /// Data object used for reader provision
        /// </summary>
        private TData DataObject { get; }

        /// <summary>
        /// Generates new data reader provider for a data object
        /// </summary>
        /// <param name="dataObject"></param>
        public DataReadProvider(TData dataObject, DataAccessLocker dataLocker) : base(dataLocker)
        {
            DataObject = dataObject ?? throw new ArgumentNullException(nameof(dataObject));
        }

        /// <summary>
        /// Creates a new model data reader for the internal data object
        /// </summary>
        /// <returns></returns>
        public DataReader<TData, TDataPort> Create()
        {
            return new DataReader<TData, TDataPort>(DataObject, DataLocker);
        }

        /// <summary>
        /// Creates new model data reader as a reader interface
        /// </summary>
        /// <returns></returns>
        public IDataReader<TDataPort> CreateInterface()
        {
            return Create();
        }
    }
}
