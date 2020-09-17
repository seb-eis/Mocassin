using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mocassin.Framework.Processing
{
    /// <summary>
    ///     Async version of the break pipeline that processes an object with the first accepting handler and returns a new
    ///     task processing the object
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class AsyncBreakPipeline<TResult> : BreakPipeline<Task<TResult>>
    {
        /// <inheritdoc />
        public AsyncBreakPipeline(IObjectProcessor<Task<TResult>> onCannotProcess)
            : base(onCannotProcess)
        {
        }

        /// <inheritdoc />
        public AsyncBreakPipeline(IObjectProcessor<Task<TResult>> onCannotProcess, List<IObjectProcessor<Task<TResult>>> handlers)
            : base(onCannotProcess, handlers)
        {
        }

        /// <inheritdoc />
        public AsyncBreakPipeline(IObjectProcessor<Task<TResult>> onCannotProcess, IEnumerable<IAsyncObjectProcessor<TResult>> handlers)
            : this(onCannotProcess)
        {
            Handlers = handlers.ToList<IObjectProcessor<Task<TResult>>>();
        }
    }
}