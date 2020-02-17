using System;
using Mocassin.Framework.Exceptions;

namespace Mocassin.Framework.Processing
{
    /// <summary>
    ///     Generic object handler class for functions that accept one argument (Do not use for tuple systems!)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class ObjectProcessor<T1, TResult> : IObjectProcessor<TResult>
    {
        /// <summary>
        ///     Delegate for handling a passed object
        /// </summary>
        protected Func<T1, TResult> ProcessDelegate { get; set; }


        /// <summary>
        ///     Static constructor that checks if the type argument of the handler is of type value tuple or tuple
        /// </summary>
        static ObjectProcessor()
        {
            if (ObjectProcessorFactory.AnyIsTuple(typeof(T1)))
            {
                throw new InvalidGenericTypeException(
                    "Object handlers cannot directly be used for tuple types due to casting issues within C#", typeof(T1));
            }
        }

        /// <summary>
        ///     Creates new object handler from a processing delegate
        /// </summary>
        public ObjectProcessor(Func<T1, TResult> handler)
        {
            ProcessDelegate = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <inheritdoc />
        public bool CanProcess(object obj, params object[] args)
        {
            return args.Length == 0 && obj is T1;
        }

        /// <inheritdoc />
        public TResult Process(object obj, params object[] args)
        {
            return ProcessDelegate((T1) obj);
        }
    }

    /// <summary>
    ///     Generic object handler class that handles an argument of specific tuple type and returns a handling result (Wraps
    ///     internally into tuples of objects)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class ObjectProcessor<T1, T2, TResult> : IObjectProcessor<TResult>
    {
        /// <summary>
        ///     Delegate for handling passed objects
        /// </summary>
        protected Func<T1, T2, TResult> ProcessDelegate { get; set; }

        /// <summary>
        ///     Creates new object handler from a processing delegate
        /// </summary>
        public ObjectProcessor(Func<T1, T2, TResult> handler)
        {
            ProcessDelegate = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <inheritdoc />
        public bool CanProcess(object obj, params object[] args)
        {
            if (args.Length != 1) return false;
            return obj is T1 && args[0] is T2;
        }

        /// <inheritdoc />
        public TResult Process(object obj, params object[] args)
        {
            return ProcessDelegate((T1) obj, (T2) args[0]);
        }
    }
}