using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using ICon.Mathematics.Extensions;

namespace ICon.Mathematics.Bitmasks
{
    /// <summary>
    /// Two wrapped 64 bit unsigned integer to be used as 128 entry bitmask (Waring: Mutable struct!)
    /// </summary>
    [Serializable]
    [XmlRoot("Mask")]
    public class Bitmask128 : IBitmask, IComparable<Bitmask128>, IEquatable<Bitmask128>
    {
        /// <summary>
        /// The first internal 64 bit mask
        /// </summary>
        [XmlAttribute("Value0")]
        public UInt64 Mask0 { get; set; }

        /// <summary>
        /// The first internal 64 bit mask
        /// </summary>
        [XmlAttribute("Value1")]
        public UInt64 Mask1 { get; set; }

        /// <summary>
        /// Creates new mask from unsiged integer
        /// </summary>
        /// <param name="mask"></param>
        public Bitmask128(UInt64 mask0, UInt64 mask1)
        {
            Mask0 = mask0;
            Mask1 = mask1;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="bitmask"></param>
        public Bitmask128(Bitmask128 bitmask)
        {
            Mask0 = bitmask.Mask0;
            Mask1 = bitmask.Mask1;
        }

        /// <summary>
        /// Access the masks true/false entries by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Boolean this[Int32 index]
        {
            get { return (index < 64) ? Mask0.GetBit(index) : Mask1.GetBit(index - 64); }
            set
            {
                if (value)
                {
                    if (index < 64)
                    {
                        Mask0 = Mask0.SetBit(index);
                    }
                    else
                    {
                        Mask1 = Mask1.SetBit(index - 64);
                    }
                }
                else
                {
                    if (index < 64)
                    {
                        Mask0 = Mask0.UnsetBit(index);
                    }
                    else
                    {
                        Mask1 = Mask1.UnsetBit(index - 64);
                    }
                }
            }
        }

        /// <summary>
        /// Compares the wrapped integer
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo(Bitmask128 other)
        {
            Int32 compare1 = Mask1.CompareTo(other.Mask1);
            if (compare1 == 0)
            {
                return Mask0.CompareTo(other.Mask0);
            }
            return compare1;
        }

        /// <summary>
        /// Compares the stored masks for bitwise equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(Bitmask128 other)
        {
            return Mask0 == other.Mask0 && Mask1 == other.Mask1;
        }

        /// <summary>
        /// Access the
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private Boolean GetEntry(UInt64 value, Int32 index)
        {
            return (value & (1U << index)) != 0U;
        }
    }
}
