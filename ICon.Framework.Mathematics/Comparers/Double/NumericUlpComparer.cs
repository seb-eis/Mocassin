using System;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Mathematics.Comparers
{
    /// <summary>
    ///     Double comparer class that performs almost equal comparisons using the ULP integer conversion principle of the IEEE
    ///     standard (not zero safe)
    /// </summary>
    public class NumericUlpComparer : NumericComparer
    {
        /// <summary>
        ///     Shared zero safe flag
        /// </summary>
        private static readonly bool ZeroSaveValue = false;

        /// <summary>
        ///     The number of allowed offset steps within the integer converted double values
        /// </summary>
        public int Steps { get; protected set; }

        /// <inheritdoc />
        public override bool IsZeroCompatible => ZeroSaveValue;

        /// <summary>
        ///     Creates a new ULP comparer with the specified number of steps
        /// </summary>
        /// <param name="steps"></param>
        public NumericUlpComparer(int steps)
        {
            Steps = Math.Abs(steps);
        }

        /// <inheritdoc />
        public override int Compare(double x, double y)
        {
            return Equals(x, y) ? 0 : x < y ? -1 : 1;
        }

        /// <inheritdoc />
        public override bool Equals(double x, double y)
        {
            return x.IsAlmostEqualByUlp(y, Steps);
        }
    }
}