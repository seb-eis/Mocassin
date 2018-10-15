using System;
using System.Threading.Tasks;

namespace ICon.Framework.Processing
{
    /// <summary>
    ///     Generic async object handler class that handles one argument of specific type in a new task (Cannot be used for
    ///     tuple types!)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class AsyncObjectProcessor<T1, TResult> : ObjectProcessor<T1, Task<TResult>>, IAsyncObjectProcessor<TResult>
    {
        /// <inheritdoc />
        public AsyncObjectProcessor(Func<T1, Task<TResult>> handler)
            : base(handler)
        {
        }

        /// <summary>
        ///     Creates new one argument async object handler from sync delegate
        /// </summary>
        /// <param name="syncHandler"></param>
        public AsyncObjectProcessor(Func<T1, TResult> syncHandler)
            : this(a => Task.Run(() => syncHandler(a)))
        {
        }
    }

    /// <summary>
    ///     Generic async object handler class that handles two arguments of specific type in a new task (Cannot be used for
    ///     tuple types!)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class AsyncObjectProcessor<T1, T2, TResult> : ObjectProcessor<T1, T2, Task<TResult>>, IAsyncObjectProcessor<TResult>
    {
        /// <inheritdoc />
        public AsyncObjectProcessor(Func<T1, T2, Task<TResult>> handler)
            : base(handler)
        {
        }

        /// <summary>
        ///     Creates new two argument async object handler from sync delegate
        /// </summary>
        /// <param name="syncHandler"></param>
        public AsyncObjectProcessor(Func<T1, T2, TResult> syncHandler)
            : base((a, b) => Task.Run(() => syncHandler(a, b)))
        {
        }
    }
}