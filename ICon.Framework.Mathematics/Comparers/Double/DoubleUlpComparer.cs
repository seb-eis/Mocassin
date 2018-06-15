using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.Extensions;

namespace ICon.Mathematics.Comparers
{
    /// <summary>
    /// Double comparer class that performs almost equal comparisons using the ULP integer conversion principle of the IEEE standard (not zero safe)
    /// </summary>
    public class DoubleUlpComparer : DoubleComparer
    {
        /// <summary>
        /// Shared zero safe flag
        /// </summary>
        private static Boolean ZeroSaveValue = false;

        /// <summary>
        /// The number of allowed offset steps within the integer converted double values
        /// </summary>
        public Int32 Steps { get; protected set; }

        /// <summary>
        /// Identifies that the comparer can savely handle zero comparisons
        /// </summary>
        public override Boolean ZeroSafe => ZeroSaveValue;

        /// <summary>
        /// Creates a new ULP comparer with the specified number of steps
        /// </summary>
        /// <param name="steps"></param>
        public DoubleUlpComparer(Int32 steps)
        {
            Steps = Math.Abs(steps);
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
            return x.AlmostEqualByULP(y, Steps);
        }
    }
}
