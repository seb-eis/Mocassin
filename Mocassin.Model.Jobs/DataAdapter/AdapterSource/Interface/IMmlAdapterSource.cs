using System.Collections.Generic;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Defines a source for data adapters that are used to exchange data between the model system and an external source
    /// </summary>
    public interface IMmlAdapterSource
    {
        /// <summary>
        ///     Get the adapter that can handle the data exchange between the two defined types
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        IDataAdapter<TData, THandler> GetAdapter<TData, THandler>();

        /// <summary>
        ///     Get the all adapters that can handle the specified data and handler types
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        IEnumerable<IDataAdapter<TData, THandler>> GetAdapters<TData, THandler>();

        /// <summary>
        ///     Get the adapter of the specified type. Method should return null if there is more than one matching adapter
        /// </summary>
        /// <typeparam name="TAdapter"></typeparam>
        /// <returns></returns>
        TAdapter GetAdapter<TAdapter>();

        /// <summary>
        ///     Get the set of adapters that matches the passed type
        /// </summary>
        /// <typeparam name="TAdapter"></typeparam>
        /// <returns></returns>
        IEnumerable<TAdapter> GetAdapters<TAdapter>();
    }
}