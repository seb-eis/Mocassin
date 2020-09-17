using System;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Represents a generic disposable model data reader that provides read only access to a data object through the
    ///     specified data port
    /// </summary>
    /// <typeparam name="TPort"></typeparam>
    public interface IDataReader<out TPort> : IDisposable where TPort : class, IModelDataPort
    {
        /// <summary>
        ///     Data access through the read only data port
        /// </summary>
        TPort Access { get; }
    }
}