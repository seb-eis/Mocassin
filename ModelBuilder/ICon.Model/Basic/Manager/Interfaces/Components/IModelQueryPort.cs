using System;
using System.Reactive;
using System.Threading.Tasks;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Base interface for all model query ports
    /// </summary>
    public interface IModelQueryPort
    {
    }

    /// <summary>
    ///     Generic base interface for all model query port interfaces that support at least delegate based custom data access
    ///     queries and provision of safe data reader interfaces
    /// </summary>
    public interface IModelQueryPort<out TPort> : IModelQueryPort
        where TPort : class, IModelDataPort
    {
        /// <summary>
        ///     Performs a read only data query using the specified data port that does not return a value
        /// </summary>
        /// <param name="action"></param>
        void Query(Action<TPort> action);

        /// <summary>
        ///     Performs a read only async data query using the specified data port that does not return a value
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        Task<Unit> AsyncQuery(Action<TPort> action);

        /// <summary>
        ///     Performs a read only data query using the specified data port that does return a value
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        TResult Query<TResult>(Func<TPort, TResult> function);

        /// <summary>
        ///     Performs a read only async data query using the specified data port that does return a value
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        Task<TResult> AsyncQuery<TResult>(Func<TPort, TResult> function);
    }
}