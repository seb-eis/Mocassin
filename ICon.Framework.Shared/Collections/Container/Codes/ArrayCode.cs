using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Mocassin.Framework.Collections
{
    /// <summary>
    ///     Defines an abstract base class wrapper for implementations of arrays that describe an encoding
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    [DataContract]
    public abstract class ArrayCode<T1> : IReadOnlyList<T1>
    {
        /// <summary>
        ///     The code array sequence
        /// </summary>
        [DataMember]
        public T1[] CodeValues { get; set; }

        /// <inheritdoc />
        [IgnoreDataMember]
        public int Count => CodeValues.Length;

        /// <inheritdoc />
        public T1 this[int indexer]
        {
            get => CodeValues[indexer];
            set => CodeValues[indexer] = value;
        }

        /// <inheritdoc />
        public IEnumerator<T1> GetEnumerator()
        {
            return CodeValues.AsEnumerable().GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return CodeValues.GetEnumerator();
        }

        /// <summary>
        ///     Copies the code sequence to another starting at the specified index
        /// </summary>
        /// <param name="other"></param>
        /// <param name="index"></param>
        public void CopyTo(ArrayCode<T1> other, int index)
        {
            CodeValues.CopyTo(other.CodeValues, index);
        }

        /// <summary>
        ///     Get the type name of the array code
        /// </summary>
        /// <returns></returns>
        public abstract string GetTypeName();

        /// <summary>
        ///     Get a JSON representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{GetTypeName()} {JsonConvert.SerializeObject(this)}";
        }
    }
}