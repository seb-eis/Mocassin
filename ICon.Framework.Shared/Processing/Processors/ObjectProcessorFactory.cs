using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mocassin.Framework.Processing
{
    /// <summary>
    ///     Factory that provides functions to create object handlers both sync and async
    /// </summary>
    public static class ObjectProcessorFactory
    {
        /// <summary>
        ///     Creates new object handler from the provided delegate
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static ObjectProcessor<T1, TResult> Create<T1, TResult>(Func<T1, TResult> handler)
        {
            return new ObjectProcessor<T1, TResult>(handler);
        }

        /// <summary>
        ///     Creates new async object handler from the provided delegate
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static AsyncObjectProcessor<T1, TResult> CreateAsync<T1, TResult>(Func<T1, TResult> handler)
        {
            return new AsyncObjectProcessor<T1, TResult>(obj => Task.Run(() => handler(obj)));
        }

        /// <summary>
        ///     Creates new object handler for two value tuple from the provided delegate
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static ObjectProcessor<T1, T2, TResult> Create<T1, T2, TResult>(Func<T1, T2, TResult> handler)
        {
            return new ObjectProcessor<T1, T2, TResult>(handler);
        }

        /// <summary>
        ///     Creates new async object handler for two value tuple from the provided delegate
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static AsyncObjectProcessor<T1, T2, TResult> CreateAsync<T1, T2, TResult>(Func<T1, T2, TResult> handler)
        {
            return new AsyncObjectProcessor<T1, T2, TResult>((a, b) => Task.Run(() => handler(a, b)));
        }

        /// <summary>
        ///     Checks a series of types for being of generic tuple or value tuple type (Does not check the base class in the
        ///     tuple case!)
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static bool AnyIsTuple(params Type[] types)
        {
            foreach (var type in types)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));

                if (!type.IsGenericType) 
                    continue;

                var genType = type.GetGenericTypeDefinition();
                if (GetGenericTupleTypes().Contains(genType) || GetGenericValueTupleTypes().Contains(genType))
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns full array of existing generic tuple types
        /// </summary>
        /// <returns></returns>
        public static Type[] GetGenericTupleTypes()
        {
            return new[]
            {
                typeof(Tuple<>),
                typeof(Tuple<,>),
                typeof(Tuple<,,>),
                typeof(Tuple<,,,>),
                typeof(Tuple<,,,,>),
                typeof(Tuple<,,,,,>),
                typeof(Tuple<,,,,,,>),
                typeof(Tuple<,,,,,,,>)
            };
        }

        /// <summary>
        ///     Returns full array of existing generic value tuple types
        /// </summary>
        /// <returns></returns>
        public static Type[] GetGenericValueTupleTypes()
        {
            return new[]
            {
                typeof(ValueTuple<>),
                typeof(ValueTuple<,>),
                typeof(ValueTuple<,,>),
                typeof(ValueTuple<,,,>),
                typeof(ValueTuple<,,,,>),
                typeof(ValueTuple<,,,,,>),
                typeof(ValueTuple<,,,,,,>),
                typeof(ValueTuple<,,,,,,,>)
            };
        }
    }
}