using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Represents a generic data source that holds model objects and model parameters
    /// </summary>
    public interface IDataSource
    {
        /// <summary>
        /// Defines the manager interface type this data source is affiliated with
        /// </summary>
        Type SourceManagerType { get; set; }

        /// <summary>
        /// Get the internal model object that matches the provided type at the specfified index (Returns null if not found)
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        T1 GetModelObject<T1>(int index) where T1 : IModelObject;

        /// <summary>
        /// Get the internal model parameter that matches the provided type (Retruns null if not found)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        T1 GetModelParameter<T1>() where T1 : IModelParameter;
    }
}
