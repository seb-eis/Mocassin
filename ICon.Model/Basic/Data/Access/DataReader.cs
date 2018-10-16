using System;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Base class for implementations of disposable model data readers
    /// </summary>
    public abstract class DataReader : IDisposable
    {
        /// <summary>
        ///     The disposable lock on the data object
        /// </summary>
        protected IDisposable Lock { get; set; }

        /// <inheritdoc />
        public void Dispose()
        {
            DisposeDataPort();
            Lock?.Dispose();
        }

        /// <summary>
        ///     Abstract method that disposes the data port
        /// </summary>
        protected abstract void DisposeDataPort();
    }

    /// <summary>
    ///     Generic disposable model data reader that locks a data object for changing operations making it read only as long
    ///     as
    ///     a non disposed reader exists
    /// </summary>
    public class DataReader<TData, TPort> : DataReader, IDataReader<TPort>
        where TData : ModelData<TPort>
        where TPort : class, IModelDataPort
    {
        /// <inheritdoc />
        public TPort Access { get; private set; }

        /// <summary>
        ///     Creates new data writer for a data object and the read only data port
        /// </summary>
        /// <param name="data"></param>
        /// <param name="lockSource"></param>
        public DataReader(TData data, AccessLockSource lockSource)
        {
            if (lockSource == null) 
                throw new ArgumentNullException(nameof(lockSource));

            Access = data.AsReadOnly();
            Lock = lockSource.TryGetReadAccess(data);
        }

        /// <inheritdoc />
        protected override void DisposeDataPort()
        {
            Access = null;
        }
    }
}