using System;

namespace Mocassin.Framework.Provider
{
    /// <summary>
    ///     Abstract base class for object providers that supplies factory functions
    /// </summary>
    public abstract class ObjectProvider
    {
        /// <summary>
        ///     Creates new object provider from and retrieval delegate
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="retrievalDelegate"></param>
        /// <returns></returns>
        public static ObjectProvider<T1> Create<T1>(Func<T1> retrievalDelegate)
        {
            return new ObjectProvider<T1>(retrievalDelegate);
        }
    }

    /// <summary>
    ///     Basic implementation for an object provider that functions on a retrieval delegate basis
    /// </summary>
    public class ObjectProvider<T1> : ObjectProvider, IObjectProvider<T1>
    {
        /// <summary>
        ///     Delegate to the source of the requested object
        /// </summary>
        protected Func<T1> RetrievalDelegate { get; set; }

        /// <summary>
        ///     Create new object provider for a retrieval delegate
        /// </summary>
        /// <param name="retrievalDelegate"></param>
        public ObjectProvider(Func<T1> retrievalDelegate)
        {
            RetrievalDelegate = retrievalDelegate ?? throw new ArgumentNullException(nameof(retrievalDelegate));
        }

        /// <inheritdoc />
        public T1 Get()
        {
            return RetrievalDelegate();
        }
    }
}