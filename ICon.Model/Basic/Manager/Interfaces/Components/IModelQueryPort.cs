using System;
using System.Threading.Tasks;
using System.Reactive;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Base interface for all model query ports
    /// </summary>
    public interface IModelQueryPort
    {

    }

    /// <summary>
    /// Generic base interface for all model query port interfaces that support at least delegate based custom data access queries and provision of safe data reader interfaces
    /// </summary>
    public interface IModelQueryPort<TDataPort> : IModelQueryPort where TDataPort : class, IModelDataPort
    {
        /// <summary>
        /// Performs a read only data query using the specified data port that does not return a value
        /// </summary>
        /// <param name="action"></param>
        void Query(Action<TDataPort> query);

        /// <summary>
        /// Performs a read only async data query using the specified data port that does not return a value
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<Unit> AsyncQuery(Action<TDataPort> query);

        /// <summary>
        /// Performs a read only data query using the specified data port that does return a value
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        TResult Query<TResult>(Func<TDataPort, TResult> query);

        /// <summary>
        /// Performs a read only async data query using the specified data port that does return a value
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TResult> AsyncQuery<TResult>(Func<TDataPort, TResult> query);
    }
}
