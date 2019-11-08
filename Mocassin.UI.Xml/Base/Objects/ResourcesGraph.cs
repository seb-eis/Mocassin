using System;
using System.Collections.Generic;
using System.Globalization;

namespace Mocassin.UI.Xml.Base
{
    /// <summary>
    ///     Serializable data class for attacking a JSON serializable resource dictionary to project graphs
    /// </summary>
    public class ResourcesGraph
    {
        private static readonly CultureInfo DefaultCultureInfo = CultureInfo.InvariantCulture;

        /// <summary>
        ///     Get or set the <see cref="Dictionary{TKey,TValue}" /> that stores resources of the project as string values (It is
        ///     advised to use the get/set helper functions to access the resource dictionary)
        /// </summary>
        public Dictionary<string, string> Content { get; set; }

        /// <summary>
        ///     Creates a new <see cref="ResourcesGraph"/> with empty dictionary
        /// </summary>
        public ResourcesGraph()
        {
            Content = new Dictionary<string, string>();
        }

        /// <summary>
        ///     Checks if the <see cref="ResourcesGraph"/> has a resource with the provided key
        /// </summary>
        /// <returns></returns>
        public bool HasResource(string key)
        {
            return Content.ContainsKey(key);
        }

                /// <summary>
        ///     Sets a generic resource of type <see cref="T"/> with the supplied key and converter <see cref="Func{T,TResult}"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="converter"></param>
        public void SetResource<T>(string key, T value, Func<T, string> converter)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (converter == null) throw new ArgumentNullException(nameof(converter));
            Content[key] = converter.Invoke(value);
        }

        /// <summary>
        ///     Sets a <see cref="string"/> resource with the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetResource(string key, string value)
        {
            SetResource(key, value, x => x);
        }

        /// <summary>
        ///     Sets a <see cref="int"/> resource with the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetResource(string key, int value)
        {
            SetResource(key, value, x => x.ToString(DefaultCultureInfo));
        }

        /// <summary>
        ///     Sets a <see cref="long"/> resource with the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetResource(string key, long value)
        {
            SetResource(key, value, x => x.ToString(DefaultCultureInfo));
        }

        /// <summary>
        ///     Sets a <see cref="double"/> resource with the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetResource(string key, double value)
        {
            SetResource(key, value, x => x.ToString(DefaultCultureInfo));
        }

        /// <summary>
        ///     Sets a <see cref="bool"/> resource with the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetResource(string key, bool value)
        {
            SetResource(key, value, x => x.ToString(DefaultCultureInfo));
        }

        /// <summary>
        ///     Tries to get a stored generic resource as type <see cref="T"/> that belongs to the supplied key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="converter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetResource<T>(string key, Func<string, T> converter, out T value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (converter == null) throw new ArgumentNullException(nameof(converter));
            if (!Content.TryGetValue(key, out var strValue))
            {
                value = default;
                return false;
            }

            value = converter.Invoke(strValue);
            return true;
        }

        /// <summary>
        ///     Tries to get a stored <see cref="string"/> resource with the supplied key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetResource(string key, out string value)
        {
            return TryGetResource(key, x => x, out value);
        }

        /// <summary>
        ///     Tries to get a stored <see cref="int"/> resource with the supplied key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetResource(string key, out int value)
        {
            return TryGetResource(key, x => int.Parse(x, DefaultCultureInfo), out value);
        }

        /// <summary>
        ///     Tries to get a stored <see cref="long"/> resource with the supplied key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetResource(string key, out long value)
        {
            return TryGetResource(key, x => long.Parse(x, DefaultCultureInfo), out value);
        }

        /// <summary>
        ///     Tries to get a stored <see cref="double"/> resource with the supplied key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetResource(string key, out double value)
        {
            return TryGetResource(key, x => double.Parse(x, DefaultCultureInfo), out value);
        }

        /// <summary>
        ///     Tries to get a stored <see cref="bool"/> resource with the supplied key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetResource(string key, out bool value)
        {
            return TryGetResource(key, bool.Parse, out value);
        }
    }
}