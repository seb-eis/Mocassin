using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Represent 'on-demand' data that supports caching
    /// </summary>
    public interface ICachedData : IEquatable<ICachedData>
    {
        /// <summary>
        /// Data creation delegate to identify the cached data object in a container
        /// </summary>
        Delegate DataCreationDelegate { get; } 

        /// <summary>
        /// Get the data as an object
        /// </summary>
        /// <returns></returns>
        object GetValue();

        /// <summary>
        /// The the data by a new task
        /// </summary>
        /// <returns></returns>
        Task<object> GetValueAsync();

        /// <summary>
        /// Flag that indicates if the data is no longer valid
        /// </summary>
        bool IsDeprecated { get; }

        /// <summary>
        /// Clears the cached data object
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears the data only if it is deprecated
        /// </summary>
        void ClearIfDeprecated();

        /// <summary>
        /// Marks the data as deprecated or no longer up to date
        /// </summary>
        void MarkAsDeprecated();
    }
}
