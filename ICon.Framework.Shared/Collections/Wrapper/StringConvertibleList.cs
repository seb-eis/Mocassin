using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ICon.Framework.Extensions;

namespace ICon.Framework.Collections
{
    /// <summary>
    /// Generic string convertible list wrapper that supplies serializing and deserializing the list into a string
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StringConvertibleList<T> : IList<T> where T : IConvertible
    {
        /// <summary>
        /// The separator used to sepraarted the values
        /// </summary>
        public char Separator { get; set; }

        /// <summary>
        /// The inner list of values
        /// </summary>
        protected List<T> Values { get; set; }

        /// <summary>
        /// Index access to the list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return Values[index];
            }

            set
            {
                Values[index] = value;
            }
        }

        /// <summary>
        /// Default construct a new string convertible list. Default separator is a comma
        /// </summary>
        public StringConvertibleList()
        {
            Separator = ',';
            Values = new List<T>();
        }

        /// <summary>
        /// Default construct new convertible list with initial capacity
        /// </summary>
        /// <param name="capacity"></param>
        public StringConvertibleList(int capacity) : this()
        {
            Values.Capacity = capacity;
        }

        /// <summary>
        /// Get the number of entries in the list
        /// </summary>
        public int Count => Values.Count;

        /// <summary>
        /// Checks if the list is read only (Always false)
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Add an item to the list
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            Values.Add(item);
        }

        /// <summary>
        /// Clear the list
        /// </summary>
        public void Clear()
        {
            Values.Clear();
        }

        /// <summary>
        /// Cechk if the list contains the provided item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return Values.Contains(item);
        }

        /// <summary>
        /// Copies the list into the passed arra starting at the provided index
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Get the generic enumerator for the list
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        /// <summary>
        /// Get the index of the provided item in the lst
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            return Values.IndexOf(item);
        }

        /// <summary>
        /// Insert the provided item into the list ath the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            Values.Insert(index, item);
        }

        /// <summary>
        /// Remove the passed item if it exists in the list
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            return Values.Remove(item);
        }

        /// <summary>
        /// Removes the item at the provided index from the list
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            Values.RemoveAt(index);
        }

        /// <summary>
        /// Get the non generic enumerator for the list
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        /// <summary>
        /// Creates a single string from the list where values are separated by the defined separator
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder(Values.Count * 10);
            builder.AppendSeparated(Separator, Values.Cast<IConvertible>());
            return builder.ToString();
        }

        /// <summary>
        /// Loads the list from a single separated value string and a converter delegate that converts a substring to the correct type
        /// </summary>
        /// <param name="linearized"></param>
        public void FromString(string linearized, Func<string,T> converter)
        {
            Values = linearized.ParseToValueList(Separator, converter);
        }
    }
}
