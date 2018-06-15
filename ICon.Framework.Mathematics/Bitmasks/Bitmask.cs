using System;

namespace ICon.Mathematics.Bitmasks
{
    /// <summary>
    /// Abstract base class for all bitmask of arbitrary size
    /// </summary>
    public interface IBitmask
    {
        /// <summary>
        /// Index access of the entries
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Boolean this[Int32 index] { get; set; }
    }
}
