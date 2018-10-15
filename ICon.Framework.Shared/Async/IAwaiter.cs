using System.Runtime.CompilerServices;

namespace ICon.Framework.Async
{
    /// <summary>
    ///     Represents an Async/Await awaiter object that returns nothing and schedules async method continuations when it
    ///     completes
    /// </summary>
    public interface IAwaiter : INotifyCompletion
    {
        /// <summary>
        ///     Indicates if the awaitable operation is completed
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        ///     Returns the result
        /// </summary>
        void GetResult();
    }

    /// <summary>
    ///     Represents an Async/Await awaiter object that returns a value and schedules async method continuations when it
    ///     completes
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IAwaiter<out TResult> : INotifyCompletion
    {
        /// <summary>
        ///     Indicates if the awaitable operation is completed
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        ///     Returns the result
        /// </summary>
        TResult GetResult();
    }
}