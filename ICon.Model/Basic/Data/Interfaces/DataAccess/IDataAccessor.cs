using System;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Represents a disposable model data accessor that provides full access to a model data object while blocking other
    ///     readers/writers from accessing the object
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IDataAccessor<TData> : IDisposable where TData : ModelData
    {
        /// <summary>
        ///     Perform an exclusive access operation that returns nothing
        /// </summary>
        /// <param name="action"></param>
        void Query(Action<TData> action);

        /// <summary>
        ///     Perform an exclusive access operation that returns a value
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        TResult Query<TResult>(Func<TData, TResult> function);

        /// <summary>
        ///     Makes reader interface for the specified data port from the data accessor
        /// </summary>
        /// <typeparam name="TPort"></typeparam>
        /// <returns></returns>
        IDataReader<TPort> AsReader<TPort>() where TPort : class, IModelDataPort;

        /// <summary>
        ///     Makes interface reader that matches the data port of the passed reader provider
        /// </summary>
        /// <typeparam name="TPort"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        IDataReader<TPort> AsReader<TPort>(IDataReaderSource<TPort> source) where TPort : class, IModelDataPort;
    }
}