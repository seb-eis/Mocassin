using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Mathematics.Comparers
{
    /// <summary>
    /// Abstract base class for all tolerance based double comparers
    /// </summary>
    public abstract class DoubleComparer : Comparer<Double>, IEqualityComparer<Double>
    {
        /// <summary>
        /// Flag that defines if the comparer can be safely used to compare to zero
        /// </summary>
        public abstract Boolean ZeroSafe { get; } 

        protected DoubleComparer()
        {

        }

        /// <summary>
        /// Creates a new ULP comparator object with the spcified step count
        /// </summary>
        /// <param name="steps"></param>
        /// <returns></returns>
        public static DoubleComparer CreateULP(Int32 steps)
        {
            return new DoubleUlpComparer(steps);
        }

        /// <summary>
        /// Creates a new relative comparator object with the specified relative factor
        /// </summary>
        /// <param name="relativeRange"></param>
        /// <returns></returns>
        public static DoubleComparer CreateRelative(Double relativeRange)
        {
            return new DoubleRelativeComparer(relativeRange);
        }

        /// <summary>
        /// Creates a new range comparator object with the specified absolute range
        /// </summary>
        /// <param name="absoluteRange"></param>
        /// <returns></returns>
        public static DoubleComparer CreateRanged(Double absoluteRange)
        {
            return new DoubleRangeComparer(absoluteRange);
        }

        /// <summary>
        /// Creates a new default tolerance comparer (range based, allows 1.0e-13 offset, overwrites Comparer.Default)
        /// </summary>
        /// <returns></returns>
        public static new DoubleComparer Default()
        {
            return CreateRanged(1.0e-13);
        }

        /// <summary>
        /// Compares two double values with the internally specified information
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public abstract override Int32 Compare(Double first, Double second);

        /// <summary>
        /// Almost equal equality comparison with the specified internal info
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public abstract Boolean Equals(Double x, Double y);

        /// <summary>
        /// Returns the default double value hash code
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Int32 GetHashCode(Double obj)
        {
            return obj.GetHashCode();
        }
    }
}
