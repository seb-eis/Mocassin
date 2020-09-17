namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Generic interface for all providers that generate safe full accessors for specific data types
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IDataAccessorSource<out TData> where TData : ModelData
    {
        /// <summary>
        ///     Creates a new disposable write interface to the data object that is thread safe
        /// </summary>
        /// <returns></returns>
        IDataAccessor<TData> Create();

        /// <summary>
        ///     Creates a new disposable write interface for the data object that is not thread safe
        /// </summary>
        /// <returns></returns>
        IDataAccessor<TData> CreateUnsafe();
    }
}