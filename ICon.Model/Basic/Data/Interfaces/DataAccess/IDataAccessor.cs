using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Represents a disposable model data accessor that provides full access to a model data object while blocking other readers/writers from accessing the object
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IDataAccessor<TData> : IDisposable where TData : ModelData
    {
        /// <summary>
        /// Perform an exclusive access operation that returns nothing
        /// </summary>
        /// <param name="action"></param>
        void Query(Action<TData> action);

        /// <summary>
        /// Perform an exlusive access operation that returns a value
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        TResult Query<TResult>(Func<TData, TResult> function);

        /// <summary>
        /// Makes reader interface for the specified data port from the data accessor
        /// </summary>
        /// <typeparam name="TDataPort"></typeparam>
        /// <returns></returns>
        IDataReader<TDataPort> AsReader<TDataPort>() where TDataPort : class, IModelDataPort;

        /// <summary>
        /// Makes interface reader that matches the data port of the passed reader provider
        /// </summary>
        /// <typeparam name="TDataPort"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        IDataReader<TDataPort> AsReader<TDataPort>(IDataReaderProvider<TDataPort> provider) where TDataPort : class, IModelDataPort;
    }
}
