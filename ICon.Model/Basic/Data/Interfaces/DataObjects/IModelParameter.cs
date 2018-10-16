using System;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Basic interface for all model parameters that are always unique and do not require deprecation or indexing
    ///     operations
    /// </summary>
    public interface IModelParameter : IEquatable<IModelParameter>
    {
        /// <summary>
        ///     Returns a string that represents the model parameter type name
        /// </summary>
        /// <returns></returns>
        string GetParameterName();
    }
}