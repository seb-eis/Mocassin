using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mocassin.Framework.Reflection
{
    /// <summary>
    ///     Delegate creator class that use reflection to search for methods that fulfill specific conditions and creates
    ///     delegates for these methods
    /// </summary>
    public class DelegateBuilder
    {
        /// <summary>
        ///     Static array of raw action types where the array index is the number of parameters
        /// </summary>
        protected static Type[] RawActionTypes { get; }

        /// <summary>
        ///     Static array of raw function types where the array index is the number of parameters
        /// </summary>
        protected static Type[] RawFunctionTypes { get; }

        /// <summary>
        ///     Static constructor that creates the action and function type lists
        /// </summary>
        static DelegateBuilder()
        {
            RawActionTypes = GetRawActionTypes();
            RawFunctionTypes = GetRawFunctionTypes();
        }

        /// <summary>
        ///     Searches the provided instance for methods that match the predicate and binding flags and returns a sequence of
        ///     delegates to these methods
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="predicate"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public IEnumerable<Delegate> CreateWhere(object instance, Predicate<MethodInfo> predicate, BindingFlags bindingFlags)
        {
            var methods = FindMethods(instance.GetType(), predicate, bindingFlags).ToList();
            var delegateTypes = MakeDelegateTypes(methods);
            return methods.Zip(delegateTypes, (methodInfo, type) => methodInfo.CreateDelegate(type, instance));
        }

        /// <summary>
        ///     Searches the provided instance for methods that match the predicate (both public and non public) and returns a
        ///     sequence of delegates to these methods
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<Delegate> CreateWhere(object instance, Predicate<MethodInfo> predicate) =>
            CreateWhere(instance, predicate, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        /// <summary>
        ///     Takes a sequence of methods infos and creates a sequence of delegates for these methods
        /// </summary>
        /// <param name="methodInfos"></param>
        /// <returns></returns>
        public IEnumerable<Type> MakeDelegateTypes(IEnumerable<MethodInfo> methodInfos) => methodInfos.Select(GetGenericDelegateType);

        /// <summary>
        ///     Finds all instance methods of a type that match the given predicate
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<MethodInfo> FindInstanceMethods(Type objType, Predicate<MethodInfo> predicate) =>
            FindMethods(objType, predicate, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        /// <summary>
        ///     Finds all instance methods of a type that match the given predicate, prefilters with binding flags
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="predicate"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public IEnumerable<MethodInfo> FindMethods(Type objType, Predicate<MethodInfo> predicate, BindingFlags bindingFlags)
        {
            return objType.GetMethods(bindingFlags).Where(info => predicate(info));
        }

        /// <summary>
        ///     Get the delegate type for the provided method info
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public Type GetGenericDelegateType(MethodInfo methodInfo)
        {
            var paramTypes = methodInfo
                             .GetParameters()
                             .Select(param => param.ParameterType)
                             .ToList();

            if (methodInfo.ReturnType == typeof(void))
                return GetRawActionType(paramTypes.Count).MakeGenericType(paramTypes.ToArray());

            paramTypes.Add(methodInfo.ReturnType);
            return GetRawFunctionType(paramTypes.Count - 1).MakeGenericType(paramTypes.ToArray());
        }

        /// <summary>
        ///     Get the raw type of an action that has the specified number of parameters
        /// </summary>
        /// <param name="paramCount"></param>
        /// <returns></returns>
        public Type GetRawActionType(int paramCount)
        {
            if (paramCount > RawActionTypes.Length)
                throw new ArgumentException("Action type has more than the max number of supported parameters", nameof(paramCount));

            return RawActionTypes[paramCount];
        }

        /// <summary>
        ///     Get the raw type of a function that has specified number of parameters
        /// </summary>
        /// <param name="paramCount"></param>
        /// <returns></returns>
        public Type GetRawFunctionType(int paramCount)
        {
            if (paramCount > RawFunctionTypes.Length)
                throw new ArgumentException("Function type has more than the max number of supported parameters", nameof(paramCount));
            return RawFunctionTypes[paramCount];
        }

        /// <summary>
        ///     Creates an array that contains the supported raw action types, each index affiliates to the number of delegate
        ///     parameters
        /// </summary>
        /// <returns></returns>
        public static Type[] GetRawActionTypes()
        {
            return new[]
            {
                typeof(Action), typeof(Action<>), typeof(Action<,>), typeof(Action<,,>), typeof(Action<,,,>), typeof(Action<,,,,>),
                typeof(Action<,,,,,>), typeof(Action<,,,,,,>), typeof(Action<,,,,,,,>), typeof(Action<,,,,,,,,,>)
            };
        }

        /// <summary>
        ///     Creates an array that contains the supported raw function types, each index affiliates to the number of delegate
        ///     parameters
        /// </summary>
        /// <returns></returns>
        public static Type[] GetRawFunctionTypes()
        {
            return new[]
            {
                typeof(Func<>), typeof(Func<,>), typeof(Func<,,>), typeof(Func<,,,>), typeof(Func<,,,,>),
                typeof(Func<,,,,,>), typeof(Func<,,,,,,>), typeof(Func<,,,,,,,>), typeof(Func<,,,,,,,,,>),
                typeof(Func<,,,,,,,,,>)
            };
        }
    }
}