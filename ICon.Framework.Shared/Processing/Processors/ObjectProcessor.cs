using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Exceptions;

namespace ICon.Framework.Processing
{
    /// <summary>
    /// Generic object handler class for functions that accept one argument (Do not use for tuple systems!)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class ObjectProcessor<T1, TResult> : IObjectProcessor<TResult>
   {
        /// <summary>
        /// Delegate for handling a passed object 
        /// </summary>
        protected Func<T1, TResult> ProcessDelegate { get; set; }


        /// <summary>
        /// Static constructor that checks if the type argument of the handler is of type value tuple or tuple
        /// </summary>
        static ObjectProcessor()
        {
            if (ObjectProcessorFactory.AnyIsTupleType(typeof(T1)))
            {
                throw new InvalidGenericTypeException("Object handlers cannot directly be used for tuple types due to casting issues within C#", typeof(T1));
            }
        }

        /// <summary>
        /// Creates new object handler from a processing delegate
        /// </summary>
        public ObjectProcessor(Func<T1, TResult> handler)
        {
            ProcessDelegate = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>
        /// Checks if the handler can process the first object, returns false if the params list contains any entries
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Boolean CanProcess(Object obj, params Object[] args)
        {
            return args.Length == 0 && obj is T1;
        }

        /// <summary>
        /// Processes the first passed argument and ignores other passed objects (Throws if object cannot be cast to correct type)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public TResult Process(Object obj, params Object[] args)
        {
            return ProcessDelegate((T1)obj);
        }
    }

    /// <summary>
    /// Generic object handler class that handles an argument of specific tuple type and returns a handling result (Wraps internally into tuples of objects)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class ObjectProcessor<T1, T2, TResult> : IObjectProcessor<TResult>
    {
        /// <summary>
        /// Delegate for handling passed objects
        /// </summary>
        protected Func<T1, T2, TResult> ProcessDelegate { get; set; }

        /// <summary>
        /// Creates new object handler from a processing delegate
        /// </summary>
        public ObjectProcessor(Func<T1, T2, TResult> handler)
        {
            ProcessDelegate = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>
        /// Checks if the handler can process the provided objects, returns false if the args list length is not equal to 1
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Boolean CanProcess(Object obj, params Object[] args)
        {
            if (args.Length != 1)
            {
                return false;
            }
            return obj is T1 && args[0] is T2;
        }

        /// <summary>
        /// Processes the first two passed obejcts, trhows if the casts to the correct types fail
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public TResult Process(Object obj, params Object[] args)
        {
            return ProcessDelegate((T1)obj, (T2)args[0]);
        }
    }
}
