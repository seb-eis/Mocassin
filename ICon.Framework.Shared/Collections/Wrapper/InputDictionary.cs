using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Collections
{
    /// <summary>
    /// Input only wrapper for a dictionary that allows to manipulate only values for already existing keys
    /// </summary>
    public class InputDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IKeyValueSetter<TKey, TValue>
    {
        /// <summary>
        /// The wrapped dictionary interface that contains the values
        /// </summary>
        protected IDictionary<TKey, TValue> ValueDictionary { get; set; }

        /// <summary>
        /// The collection of keys
        /// </summary>
        public ICollection<TKey> Keys => ValueDictionary.Keys;

        /// <summary>
        /// The collection of values
        /// </summary>
        public ICollection<TValue> Values => ValueDictionary.Values;

        /// <summary>
        /// Creates an input wrapper around the passed value dictionary
        /// </summary>
        /// <param name="valueDictionary"></param>
        public InputDictionary(IDictionary<TKey, TValue> valueDictionary)
        {
            ValueDictionary = valueDictionary ?? throw new ArgumentNullException(nameof(valueDictionary));
        }

        /// <summary>
        /// Get the enumerator for the dictionary
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ValueDictionary.GetEnumerator();
        }

        /// <summary>
        /// Get the enumerator for the dictionary
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ValueDictionary.GetEnumerator();
        }

        /// <summary>
        /// Tries to set a value. If the key does not exists the method returns false
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TrySetValue(TKey key, TValue value)
        {
            if (Keys.Contains(key))
            {
                ValueDictionary[key] = value;
                return true;
            }
            return false;
        }
    }
}
