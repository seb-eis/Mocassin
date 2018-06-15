using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Mathematics.Extensions;

namespace ICon.Mathematics.Bitmasks
{
    /// <summary>
    /// Wrapped 64 bit unsigned integer to be used as 64 entry bitmask (Waring: Mutable struct!)
    /// </summary>
    [DataContract]
    public struct Bitmask64 : IBitmask, IComparable<Bitmask64>, IEquatable<Bitmask64>
    {
        /// <summary>
        /// The internal mask value
        /// </summary>
        [DataMember]
        public UInt64 Mask { get; set; }

        /// <summary>
        /// Get the size of the mask
        /// </summary>
        [IgnoreDataMember]
        public Int32 MaxEntries => 64;

        /// <summary>
        /// Creates new mask from unsiged integer
        /// </summary>
        /// <param name="mask"></param>
        public Bitmask64(UInt64 mask)
        {
            Mask = mask;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="bitmask"></param>
        public Bitmask64(Bitmask64 bitmask)
        {
            Mask = bitmask.Mask;
        }

        /// <summary>
        /// Access the masks true/false entries by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Boolean this[Int32 index]
        {
            get { return Mask.GetBit(index); }
            set { Mask = (value) ? Mask.SetBit(index) : Mask.UnsetBit(index); }
        }

        /// <summary>
        /// Compares the wrapped integer
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo(Bitmask64 other)
        {
            return Mask.CompareTo(other.Mask);
        }

        /// <summary>
        /// Compares the stored masks for bitwise equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(Bitmask64 other)
        {
            return Mask == other.Mask;
        }

        /// <summary>
        /// Counts the number of set bits
        /// </summary>
        /// <returns></returns>
        public Int32 PopCount()
        {
            return Mask.PopCount();
        }


        /// <summary>
        /// Implicit conversion operator for unsigned 64 bit integer
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Bitmask64(UInt64 value)
        {
            return new Bitmask64(value);
        }

        /// <summary>
        /// String representation fo the bitmask
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Bitmask ({ Mask.ToString()})";
        }
    }
}
