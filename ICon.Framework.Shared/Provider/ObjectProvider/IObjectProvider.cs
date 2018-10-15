using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Provider
{
    /// <summary>
    /// Represents an object provider for a generic object and hides the origin of the object
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public interface IObjectProvider<out T1>
    {
        /// <summary>
        /// Get the provided object
        /// </summary>
        /// <returns></returns>
        T1 Get();
    }
}
