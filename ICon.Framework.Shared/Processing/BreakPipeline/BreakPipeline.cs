using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ICon.Framework.Processing
{
    /// <summary>
    /// Abstract generic processing pipeline that supplies a set of handlers and walks through pipeline until a handler accepts the object
    /// </summary>
    public class BreakPipeline<TResult> : IObjectProcessor<TResult>
    {
        /// <summary>
        /// The list of known processing handlers in order of execution
        /// </summary>
        public List<IObjectProcessor<TResult>> Handlers { get; set; }

        /// <summary>
        /// Handler that is called if the processing of the object fails
        /// </summary>
        protected IObjectProcessor<TResult> OnCannotProcess { get; set; }

        /// <summary>
        /// Creates new break pipeline with sepcififed handler for processing failure
        /// </summary>
        /// <param name="onCannotProcess"></param>
        public BreakPipeline(IObjectProcessor<TResult> onCannotProcess)
        {
            OnCannotProcess = onCannotProcess ?? throw new ArgumentNullException(nameof(onCannotProcess));
        }

        /// <summary>
        /// Creates new break pipeline with handler list and handler for processing failure
        /// </summary>
        /// <param name="handlers"></param>
        /// <param name="onCannotProcess"></param>
        public BreakPipeline(IObjectProcessor<TResult> onCannotProcess, List<IObjectProcessor<TResult>> handlers) : this(onCannotProcess)
        {
            Handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        }

        /// <summary>
        /// Checks if any handler can process the provided set of objects
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Boolean CanProcess(Object obj, params Object[] args)
        {
            return Handlers.Any(handler => handler.CanProcess(obj, args));
        }

        /// <summary>
        /// Sends the provided argument set through the processing pipeline, if no one accepts the obejcts the cannot process delegate is called
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public TResult Process(Object obj, params Object[] args)
        {
            return (Handlers.Find(handler => handler.CanProcess(obj, args)) ?? OnCannotProcess).Process(obj, args);
        }
    }
}
