using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Async
{
    /// <summary>
    /// Represent an asynchronous awaitable sequence or object that supports the compiler 'await' statement and supplies nothing on completion
    /// </summary>
    public interface IAwaitable
    {
        /// <summary>
        /// Returns new awaiter for async/await that awaits a result of type Void
        /// </summary>
        /// <returns></returns>
        IAwaiter GetAwaiter();
    }

    /// <summary>
    /// Represent an asynchronous awaitable sequence or object that supports the compiler 'await' statement and supplies a value on completion
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IAwaitable<out TResult>
    {
        /// <summary>
        /// Returns new awaiter for async/await that awaits a result of type TResult
        /// </summary>
        /// <returns></returns>
        IAwaiter<TResult> GetAwaiter();
    }
}
