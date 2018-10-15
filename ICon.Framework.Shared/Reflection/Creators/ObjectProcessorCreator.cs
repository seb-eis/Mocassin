using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ICon.Framework.Processing;

namespace ICon.Framework.Reflection
{
    /// <summary>
    ///     Supplies automatic analysis of class instances to create sets of object processors for pipeline constructs
    /// </summary>
    public class ObjectProcessorCreator
    {
        /// <summary>
        ///     Static array of the raw processor types where the index is the number of arguments (0 is always null)
        /// </summary>
        protected static Type[] RawProcessorTypes { get; }

        /// <summary>
        ///     Static array of the raw async processor types where the index is the number of arguments (0 is always null)
        /// </summary>
        protected static Type[] RawAsyncProcessorTypes { get; }

        static ObjectProcessorCreator()
        {
            RawProcessorTypes = GetRawProcessorTypes();
            RawAsyncProcessorTypes = GetRawAsyncProcessorTypes();
        }

        /// <summary>
        ///     Searches the provided class instance for methods that match the predicate and binding flags and returns a sequence
        ///     of object processors for all found methods
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="instance"></param>
        /// <param name="predicate"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public IEnumerable<IObjectProcessor<TResult>> CreateProcessors<TResult>(object instance, Predicate<MethodInfo> predicate,
            BindingFlags bindingFlags)
        {
            return new DelegateCreator().CreateDelegates(instance, predicate, bindingFlags)
                .Select(@delegate => (IObjectProcessor<TResult>) CreateProcessor(@delegate));
        }

        /// <summary>
        ///     Searches the provided class instance for methods that match the predicate and binding flags and returns a sequence
        ///     of sync object processors fo all found methods
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="instance"></param>
        /// <param name="predicate"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public IEnumerable<IAsyncObjectProcessor<TResult>> CreateAsyncProcessors<TResult>(object instance, Predicate<MethodInfo> predicate,
            BindingFlags bindingFlags)
        {
            return new DelegateCreator().CreateDelegates(instance, predicate, bindingFlags)
                .Select(@delegate => (IAsyncObjectProcessor<TResult>) CreateAsyncProcessor(@delegate));
        }

        /// <summary>
        ///     Creates an object processor for the provided delegate
        /// </summary>
        /// <param name="delegate"></param>
        /// <returns></returns>
        protected object CreateProcessor(Delegate @delegate)
        {
            var paramTypes = @delegate.Method
                .GetParameters()
                .Select(param => param.ParameterType)
                .ToList();

            paramTypes.Add(@delegate.Method.ReturnType);
            return Activator.CreateInstance(GetRawProcessorType(paramTypes.Count - 1).MakeGenericType(paramTypes.ToArray()), @delegate);
        }

        /// <summary>
        ///     Creates an async object processor for the provided delegate type
        /// </summary>
        /// <param name="delegate"></param>
        /// <returns></returns>
        protected object CreateAsyncProcessor(Delegate @delegate)
        {
            var paramTypes = @delegate.Method
                .GetParameters()
                .Select(param => param.ParameterType)
                .ToList();

            paramTypes.Add(@delegate.Method.ReturnType);
            return Activator.CreateInstance(GetRawAsyncProcessorType(paramTypes.Count - 1).MakeGenericType(paramTypes.ToArray()),
                @delegate);
        }

        /// <summary>
        ///     Get the raw type for an object processor with the specified number of handled objects
        /// </summary>
        /// <param name="objectCount"></param>
        /// <returns></returns>
        public Type GetRawProcessorType(int objectCount)
        {
            if (objectCount > RawProcessorTypes.Length || objectCount == 0)
                throw new ArgumentException("Object count is not supported as an object processor", nameof(objectCount));

            return RawProcessorTypes[objectCount];
        }

        /// <summary>
        ///     Get the raw type for an async object processor with the specified number of handled objects
        /// </summary>
        /// <param name="objectCount"></param>
        /// <returns></returns>
        public Type GetRawAsyncProcessorType(int objectCount)
        {
            if (objectCount > RawAsyncProcessorTypes.Length || objectCount == 0)
                throw new ArgumentException("Object count is not supported as an object processor", nameof(objectCount));
            return RawAsyncProcessorTypes[objectCount];
        }

        /// <summary>
        ///     Get an array that contains the supported raw processor types (index equals number of handled objects, index 0 is
        ///     always null)
        /// </summary>
        /// <returns></returns>
        public static Type[] GetRawProcessorTypes()
        {
            return new[]
            {
                null, typeof(ObjectProcessor<,>), typeof(ObjectProcessor<,,>)
            };
        }

        /// <summary>
        ///     Get an array that contains the supported raw async processor types (index equals number of handled objects, index 0
        ///     is always null)
        /// </summary>
        /// <returns></returns>
        public static Type[] GetRawAsyncProcessorTypes()
        {
            return new[]
            {
                null, typeof(AsyncObjectProcessor<,>), typeof(AsyncObjectProcessor<,,>)
            };
        }
    }
}