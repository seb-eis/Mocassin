using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Attribute that marks a method as cacheable and defines the wrapper used for data chaning
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheableMethodAttribute : Attribute
    {
        /// <summary>
        /// The type of the cached object wrapper
        /// </summary>
        public Type GenericDataWrapperType { get; private set; }

        /// <summary>
        /// Creates the default version that uses the "AutoCachedData" wrapper for the data object
        /// </summary>
        public CacheableMethodAttribute()
        {
            GenericDataWrapperType = typeof(AutoCachedData<>);
        }

        /// <summary>
        /// Creates new cacheable method attribute where the generic type of the caching wrapper is defined
        /// </summary>
        /// <param name="genericDataWrapperType"></param>
        public CacheableMethodAttribute(Type genericDataWrapperType)
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
