using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Base class for implementations of disposable model data readers
    /// </summary>
    public abstract class DataReader : IDisposable
    {
        /// <summary>
        /// The disposable lock on the data object
        /// </summary>
        protected IDisposable Lock { get; set; }

        /// <summary>
        /// The dispose method that ensures lock release and destroys the internal data port making further access impossible
        /// </summary>
        public void Dispose()
        {
            DisposeDataPort();
            Lock?.Dispose();
        }

        /// <summary>
        /// Abstract method that disposes the data port
        /// </summary>
        protected abstract void DisposeDataPort();


    }

    /// <summary>
    /// Genric disposbale model data reader that locks a data object for changing operations making it read only as long as a non diposed reader exists
    /// </summary>
    public class DataReader<TData, TDataPort> : DataReader, IDataReader<TDataPort> where TData : ModelData<TDataPort> where TDataPort : class, IModelDataPort
    {
        /// <summary>
        /// The data object used for reading
        /// </summary>
        private TData Data { get; }

        /// <summary>
        /// The data port wrapper that provide read only access to the locked data objects fields and properties
        /// </summary>
        public TDataPort Access { get; private set; }

        /// <summary>
        /// Creates new data writer for a data object and the read only data port
        /// </summary>
        /// <param name="data"></param>
        public DataReader(TData data, DataAccessLocker locker)
        {
            if (locker == null)
            {
                throw new ArgumentNullException(nameof(locker));
            }

            Data = data ?? throw new ArgumentNullException(nameof(data));
            Access = data.AsReadOnly();
            Lock = locker.TryGetReadOnlyLock(data);
        }

        /// <summary>
        /// Destroys the data port prohibiting further usage of the data reader
        /// </summary>
        protected override void DisposeDataPort()
        {
            Access = null;
        }
    }
}
