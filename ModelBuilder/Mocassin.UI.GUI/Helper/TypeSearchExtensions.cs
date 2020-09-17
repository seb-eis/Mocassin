using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mocassin.UI.GUI.Helper
{
    /// <summary>
    ///     Extension class that provides methods for <see cref="Assembly" /> and <see cref="Type" /> searches
    /// </summary>
    public static class TypeSearchExtensions
    {
        /// <summary>
        ///     Searches a set of <see cref="Type" /> for attributed types and returns the affiliated
        ///     <see cref="KeyValuePair{TKey,TValue}" /> set
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="types"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<Type, TAttribute>> SearchAttributedTypes<TAttribute>(this IEnumerable<Type> types)
            where TAttribute : Attribute
        {
            foreach (var type in types)
            {
                if (type.GetCustomAttribute<TAttribute>() is { } attribute)
                    yield return new KeyValuePair<Type, TAttribute>(type, attribute);
            }
        }

        /// <summary>
        ///     Searches an <see cref="Assembly" /> for attributed types and returns the affiliated
        ///     <see cref="KeyValuePair{TKey,TValue}" /> set
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="onlyExported"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<Type, TAttribute>> SearchAttributedTypes<TAttribute>(this Assembly assembly,
            bool onlyExported = false)
            where TAttribute : Attribute
        {
            var types = onlyExported ? assembly.GetExportedTypes() : assembly.GetTypes();
            return types.SearchAttributedTypes<TAttribute>();
        }

        /// <summary>
        ///     Searches an assembly for <see cref="Type" /> for <see cref="Attribute" /> marked instances, creates instances and
        ///     returns the set
        ///     of <see cref="KeyValuePair{TKey,TValue}" /> of instances and attributes
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="constructorArgs"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<TObj, TAttribute>> MakeAttributedInstances<TObj, TAttribute>(this Assembly assembly,
            params object[] constructorArgs)
            where TAttribute : Attribute
        {
            foreach (var pair in assembly.SearchAttributedTypes<TAttribute>())
            {
                var instance = (TObj) Activator.CreateInstance(pair.Key, constructorArgs);
                yield return new KeyValuePair<TObj, TAttribute>(instance, pair.Value);
            }
        }
    }
}