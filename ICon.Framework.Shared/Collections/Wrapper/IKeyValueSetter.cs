using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Collections
{
    /// <summary>
    /// Represents a setter access for key value information that allows only to set existing values
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IKeyValueSetter<TKey, TValue>
    {
        /// <summary>
        /// Get the collection of existing keys
        /// </summary>
        ICollection<TKey> Keys { get; }

        /// <summary>
        /// Get the collection of existing values
        /// </summary>
        ICollection<TValue> Values { get; }

        /// <summary>
        /// Tries to set a value if the key already exists
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TrySetValue(TKey key, TValue value);
    }
}
