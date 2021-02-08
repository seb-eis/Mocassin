using System;
using System.Collections;
using System.Collections.Generic;

namespace Mocassin.Framework.Collections
{
    namespace Mocassin.Tools.Evaluation.Queries
    {
        /// <summary>
        ///     A fast read-only wrapper for the <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class ReadOnlyList<T> : IReadOnlyList<T>
        {
            private List<T> Data { get; }

            /// <inheritdoc />
            public IEnumerator<T> GetEnumerator() => Data.GetEnumerator();

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) Data).GetEnumerator();

            /// <inheritdoc />
            public int Count => Data.Count;

            /// <inheritdoc />
            public T this[int index] => Data[index];

            /// <summary>
            ///     Generate new <see cref="ReadOnlyList{T}"/> wrapper around a <see cref="List{T}"/>
            /// </summary>
            /// <param name="data"></param>
            public ReadOnlyList(List<T> data)
            {
                Data = data ?? throw new ArgumentNullException(nameof(data));
            }
        }
    }
}