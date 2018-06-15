using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections;
using Newtonsoft.Json;

namespace ICon.Framework.Collections
{
    /// <summary>
    /// Defines an abstract base class wrapper for implementations of arrays that describe an encoding
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    [DataContract]
    public abstract class ArrayCode<T1> : IReadOnlyList<T1>
    {
        /// <summary>
        /// The code array seqeunce
        /// </summary>
        [DataMember]
        public T1[] CodeValues { get; set; }

        /// <summary>
        /// The length of the code
        /// </summary>
        [IgnoreDataMember]
        public int Count => CodeValues.Length;

        /// <summary>
        /// Index access to the code
        /// </summary>
        /// <param name="indexer"></param>
        /// <returns></returns>
        public T1 this[int indexer]
        {
            get { return CodeValues[indexer]; }
            set { CodeValues[indexer] = value; }
        }

        /// <summary>
        /// Get the enumerator for the code sequence
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T1> GetEnumerator()
        {
            return CodeValues.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Get the enumerator for the code sequence
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return CodeValues.GetEnumerator();
        }

        /// <summary>
        /// Copies the code sequnce to another code seqeunce starting at the specified index
        /// </summary>
        /// <param name="other"></param>
        public void CopyTo(ArrayCode<T1> other, int index)
        {
            CodeValues.CopyTo(other.CodeValues, index);
        }

        /// <summary>
        /// Get the type name of the array code
        /// </summary>
        /// <returns></returns>
        public abstract string GetTypeName();

        /// <summary>
        /// Get a JSON representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{GetTypeName()} {JsonConvert.SerializeObject(this)}";
        }
    }
}
