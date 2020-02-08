using System;
using System.Threading.Tasks;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Represent an 'on-demand' data object source that supports object caching and a creation delegate
    /// </summary>
    public interface ICachedObjectSource : IEquatable<ICachedObjectSource>
    {
        /// <summary>
        ///     Data creation delegate to identify the cached data object in a container
        /// </summary>
        Delegate FactoryDelegate { get; }

        /// <summary>
        ///     Flag that indicates if the data is no longer valid
        /// </summary>
        bool IsDeprecated { get; }

        /// <summary>
        ///     Get the data as an object
        /// </summary>
        /// <returns></returns>
        object GetValue();

        /// <summary>
        ///     The the data by a new task
        /// </summary>
        /// <returns></returns>
        Task<object> GetValueAsync();

        /// <summary>
        ///     Clears the cached data object
        /// </summary>
        void Clear();

        /// <summary>
        ///     Clears the data only if it is deprecated
        /// </summary>
        void ClearIfDeprecated();

        /// <summary>
        ///     Marks the data as deprecated or no longer up to date
        /// </summary>
        void MarkAsDeprecated();
    }
}