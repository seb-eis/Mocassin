using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Attribute to instruct a cache manager to ad a method to its caching system
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheMethodResultAttribute : Attribute
    {
        /// <summary>
        /// The type of the cached object wrapper
        /// </summary>
        public Type GenericDataWrapperType { get; }

        /// <summary>
        /// Creates the default version that uses the "AutoCachedData" wrapper for the data object
        /// </summary>
        public CacheMethodResultAttribute()
        {
            GenericDataWrapperType = typeof(AutoCachedData<>);
        }

        /// <summary>
        /// Creates new attribute where the generic type of the caching wrapper is defined
        /// </summary>
        /// <param name="genericDataWrapperType"></param>
        public CacheMethodResultAttribute(Type genericDataWrapperType)
        {
            if (genericDataWrapperType == null)
            {
                throw new ArgumentNullException(nameof(genericDataWrapperType));
            }

            if (genericDataWrapperType.IsGenericType == false)
            {
                throw new ArgumentException("Wrapper type is not generic", nameof(genericDataWrapperType));
            }
            GenericDataWrapperType = genericDataWrapperType;
        }
    }
}
