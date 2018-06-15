﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Generic interface for all providers that generate safe full accessors for specific data types
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IDataAccessorProvider<TData> where TData : ModelData
    {
        /// <summary>
        /// Creates a new disposable write interface to the data object
        /// </summary>
        /// <returns></returns>
        IDataAccessor<TData> CreateInterface();
    }
}
