using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Mocassin.Framework.Extensions;

namespace Mocassin.Framework.Collections
{
    /// <summary>
    /// Generic string convertible list wrapper that supplies serializing and deserializing the list into a string in a csv style
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CsvSerializableList<T> : IList<T>
    {
        /// <summary>
        /// The separator for the values
        /// </summary>
        public char Separator { get; set; }

        /// <summary>
        /// The inner list of values
        /// </summary>
        protected List<T> Values { get; set; }

        /// <inheritdoc />
        public T this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }

        /// <summary>
        /// Default construct a new string convertible list. Default separator is a comma
        /// </summary>
        public CsvSerializableList()
        {
            Separator = ',';
            Values = new List<T>();
        }

        /// <summary>
        /// Default construct new convertible list with initial capacity
        /// </summary>
        /// <param name="capacity"></param>
        public CsvSerializableList(int capacity) : this()
        {
            Values.Capacity = capacity;
        }

        /// <inheritdoc />
        public int Count => Values.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public void Add(T item)
        {
            Values.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            Values.Clear();
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            return Values.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            Values.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            return Values.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, T item)
        {
            Values.Insert(index, item);
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            return Values.Remove(item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            Values.RemoveAt(index);
        }

        /// <inheritdoc />
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
            var charsPerEntry = 1;
            if (Values.Count != 0)
                charsPerEntry = Values[0].ToString().Length;

            var builder = new StringBuilder(Values.Count * (1 + charsPerEntry));
            builder.AppendSeparatedToString(Values, Separator);
            return builder.ToString();
        }

        /// <summary>
        /// Loads the list from a single separated value string and a converter delegate that converts a substring to the correct type
        /// </summary>
        /// <param name="linearized"></param>
        /// <param name="converter"></param>
        public void FromString(string linearized, Func<string, T> converter)
        {
            Values = linearized.ParseToValueList(converter, Separator);
        }

        /// <summary>
        /// Parse a string of separated values into a csv list of the specified type using the provided string to value converter and separator
        /// </summary>
        /// <param name="linearized"></param>
        /// <param name="converter"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static CsvSerializableList<T> Parse(string linearized, Func<string, T> converter, char separator)
        {
            var list = new CsvSerializableList<T>() { Separator = separator };
            list.FromString(linearized, converter);
            return list;
        }

        /// <summary>
        /// Parses a string of separated values into a csv list of the specified type using the provided converter and default separator ','
        /// </summary>
        /// <param name="linearized"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static CsvSerializableList<T> Parse(string linearized, Func<string, T> converter)
        {
            return Parse(linearized, converter, ',');
        }
    }
}
