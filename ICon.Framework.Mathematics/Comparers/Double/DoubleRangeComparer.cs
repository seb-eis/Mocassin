using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.Extensions;

namespace ICon.Mathematics.Comparers
{
    /// <summary>
    /// Double range comparer class that performs almost equal comparisons with an absolut range tolerance
    /// </summary>
    public class DoubleRangeComparer : DoubleComparer
    {
        /// <summary>
        /// Shared zero safe flag
        /// </summary>
        private static Boolean ZeroSaveValue = true;

        /// <summary>
        /// The absolute tolerance range value
        /// </summary>
        public Double Range { get; protected set; }

        /// <summary>
        /// Identifies that the comparer can savely handle zero comparisons
        /// </summary>
        public override Boolean ZeroSafe => ZeroSaveValue;

        /// <summary>
        /// Creates a new range comparer with the specified range
        /// </summary>
        /// <param name="range"></param>
        public DoubleRangeComparer(Double range)
        {
            Range = Math.Abs(range);
        }

        /// <summary>
        /// Performs compare with range (zero safe)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override Int32 Compare(Double x, Double y)
        {
            return (Equals(x, y)) ? 0 : (x < y) ? -1 : 1;
        }

        /// <summary>
        /// Checks for equality with range tolerance (zero safe check)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override Boolean Equals(Double x, Double y)
        {
            return x.AlmostEqualByRange(y, Range);
        }
    }
}
