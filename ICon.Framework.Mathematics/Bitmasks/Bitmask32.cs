using System;
using System.Xml.Serialization;
using System.Globalization;

using ICon.Mathematics.Extensions;

namespace ICon.Mathematics.Bitmasks
{
    /// <summary>
    /// Wrapped 32 bit unsigned integer to be used as 32 entry bitmask (Waring: Mutable struct!)
    /// </summary>
    [Serializable]
    [XmlRoot("Mask")]
    public struct Bitmask32 : IBitmask, IComparable<Bitmask32>, IEquatable<Bitmask32>
    {
        /// <summary>
        /// The internal mask value
        /// </summary>
        [XmlAttribute("Value")]
        public UInt32 Mask { get; set; }

        /// <summary>
        /// Creates new mask from unsiged integer
        /// </summary>
        /// <param name="mask"></param>
        public Bitmask32(UInt32 mask)
        {
            Mask = mask;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="bitmask"></param>
        public Bitmask32(Bitmask32 bitmask)
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
        /// Implicit conversion operator for unsigned 32 bit integer
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Bitmask32(UInt32 value)
        {
            return new Bitmask32(value);
        }

        /// <summary>
        /// Compares the wrapped integer
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo(Bitmask32 other)
        {
            return Mask.CompareTo(other.Mask);
        }

        /// <summary>
        /// Compares the stored masks for bitwise equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(Bitmask32 other)
        {
            return Mask == other.Mask;
        }
    }
}
