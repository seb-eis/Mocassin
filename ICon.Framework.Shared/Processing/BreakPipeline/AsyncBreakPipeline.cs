using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace ICon.Framework.Processing
{
    /// <summary>
    /// Async version of the break pipeline that processes an object with the first accepting handler and returns a new task processing the object
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class AsyncBreakPipeline<TResult> : BreakPipeline<Task<TResult>>
    {
        /// <summary>
        /// Creates new async break pipeline with the specififed processing failure handler
        /// </summary>
        /// <param name="onCannotProcess"></param>
        public AsyncBreakPipeline(IObjectProcessor<Task<TResult>> onCannotProcess) : base(onCannotProcess)
        {

        }

        /// <summary>
        /// Creates new async break pipeline with the specififed processing failure handler and initial handler list
        /// </summary>
        /// <param name="onCannotProcess"></param>
        /// <param name="handlers"></param>
        public AsyncBreakPipeline(IObjectProcessor<Task<TResult>> onCannotProcess, List<IObjectProcessor<Task<TResult>>> handlers) : base(onCannotProcess, handlers)
        {

        }

        /// <summary>
        /// Creates new async break pipeline with the specififed async processing failure handler and initial handler list
        /// </summary>
        /// <param name="onCannotProcess"></param>
        /// <param name="handlers"></param>
        public AsyncBreakPipeline(IObjectProcessor<Task<TResult>> onCannotProcess, List<IAsyncObjectProcessor<TResult>> handlers) : this(onCannotProcess)
        {
            Handlers = handlers.ToList<IObjectProcessor<Task<TResult>>>();
        }
    }
}
