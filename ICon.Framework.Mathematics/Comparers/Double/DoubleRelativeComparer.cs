using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.Extensions;

namespace ICon.Mathematics.Comparers
{
    /// <summary>
    /// Double relative comparer class that performs almost equal comparisons with a relative factor (Not zero safe)
    /// </summary>
    public class DoubleRelativeComparer : DoubleComparer
    {
        /// <summary>
        /// Shared zero safe flag
        /// </summary>
        private static Boolean ZeroSaveValue = false;

        /// <summary>
        /// The absolute tolerance range value
        /// </summary>
        public Double RelativeRange { get; protected set; }

        /// <summary>
        /// Identifies that the comparer can savely handle zero comparisons
        /// </summary>
        public override Boolean ZeroSafe => ZeroSaveValue;

        /// <summary>
        /// Creates a new relative range comparer with the specified tolerance factor
        /// </summary>
        /// <param name="range"></param>
        public DoubleRelativeComparer(Double range)
        {
            RelativeRange = Math.Abs(range);
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
            return x.AlmostEqualByRelativeRange(y, RelativeRange);
        }
    }
}
