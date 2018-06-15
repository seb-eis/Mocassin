using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Processing
{
    /// <summary>
    /// Represents an handler for objects that processes passed values and returns the result if the object could be processed
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IObjectProcessor<out TResult>
    {
        /// <summary>
        /// Checks if the handler can process the provided set of objects
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        Boolean CanProcess(Object obj, params Object[] args);

        /// <summary>
        /// Handles the passed set of objects, behvior on failure is specififed by the implementation
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        TResult Process(Object obj, params Object[] args);
    }
}
