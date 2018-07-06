using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Provider
{
    /// <summary>
    /// Abstract base class for object providers that supplies factory functions
    /// </summary>
    public abstract class ObjectProvider
    {
        /// <summary>
        /// Creates new object provider from and retrival delegate
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="retrivalDelegate"></param>
        /// <returns></returns>
        public static ObjectProvider<T1> Create<T1>(Func<T1> retrivalDelegate)
        {
            return new ObjectProvider<T1>(retrivalDelegate);
        }
    }

    /// <summary>
    /// Basic implementation for an object provider that functions on a retrival delegate basis
    /// </summary>
    public class ObjectProvider<T1> : ObjectProvider, IObjectProvider<T1>
    {
        /// <summary>
        /// Delegate to the source of the requested object
        /// </summary>
        protected Func<T1> RetrivalDelegate { get; set; }

        /// <summary>
        /// Create new object provider for a retrival delegate
        /// </summary>
        /// <param name="retrivalDelegate"></param>
        public ObjectProvider(Func<T1> retrivalDelegate)
        {
            RetrivalDelegate = retrivalDelegate ?? throw new ArgumentNullException(nameof(retrivalDelegate));
        }

        /// <summary>
        /// Retrieve the current object
        /// </summary>
        /// <returns></returns>
        public T1 Get()
        {
            return RetrivalDelegate();
        }
    }
}
