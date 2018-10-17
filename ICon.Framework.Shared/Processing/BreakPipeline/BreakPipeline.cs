using System;
using System.Collections.Generic;
using System.Linq;

namespace Mocassin.Framework.Processing
{
    /// <summary>
    ///     Abstract generic processing pipeline that supplies a set of handlers and walks through pipeline until a handler
    ///     accepts the object
    /// </summary>
    public class BreakPipeline<TResult> : IObjectProcessor<TResult>
    {
        /// <summary>
        ///     The list of known processing handlers in order of execution
        /// </summary>
        public List<IObjectProcessor<TResult>> Handlers { get; set; }

        /// <summary>
        ///     Handler that is called if the processing of the object fails
        /// </summary>
        protected IObjectProcessor<TResult> OnCannotProcess { get; set; }

        /// <summary>
        ///     Creates new break pipeline with respecified handler for processing failure
        /// </summary>
        /// <param name="onCannotProcess"></param>
        public BreakPipeline(IObjectProcessor<TResult> onCannotProcess)
        {
            OnCannotProcess = onCannotProcess ?? throw new ArgumentNullException(nameof(onCannotProcess));
        }

        /// <summary>
        ///     Creates new break pipeline with handler list and handler for processing failure
        /// </summary>
        /// <param name="handlers"></param>
        /// <param name="onCannotProcess"></param>
        public BreakPipeline(IObjectProcessor<TResult> onCannotProcess, List<IObjectProcessor<TResult>> handlers)
            : this(onCannotProcess)
        {
            Handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        }

        /// <inheritdoc />
        public bool CanProcess(object obj, params object[] args)
        {
            return Handlers.Any(handler => handler.CanProcess(obj, args));
        }

        /// <inheritdoc />
        public TResult Process(object obj, params object[] args)
        {
            return (Handlers.Find(handler => handler.CanProcess(obj, args)) ?? OnCannotProcess).Process(obj, args);
        }
    }
}