using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Generic interface for all reader providers that supply safe disposable data readers for model data objects
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TDataPort"></typeparam>
    public interface IDataReaderProvider<TDataPort> where TDataPort : class, IModelDataPort
    {
        /// <summary>
        /// Creates a new disposable model data reader interface
        /// </summary>
        /// <returns></returns>
        IDataReader<TDataPort> CreateInterface();
    }
}
