using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Represents a generic disposable model data reader that provides read only access to a data object through the specified data port
    /// </summary>
    /// <typeparam name="TDataPort"></typeparam>
    public interface IDataReader<TDataPort> : IDisposable where TDataPort : class, IModelDataPort
    {
        /// <summary>
        /// Data access through the read only data port
        /// </summary>
        TDataPort Access { get; }
    }
}
