using System;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Mathematics.Comparers
{
    /// <summary>
    ///     Double range comparer class that performs almost equal comparisons with an absolut range tolerance
    /// </summary>
    public class NumericRangeComparer : NumericComparer
    {
        /// <summary>
        ///     Shared zero safe flag
        /// </summary>
        private const bool ZeroSaveValue = true;

        /// <summary>
        ///     The absolute tolerance range value
        /// </summary>
        public double Range { get; protected set; }

        /// <inheritdoc />
        public override bool IsZeroCompatible => ZeroSaveValue;

        /// <summary>
        ///     Creates a new range comparer with the specified range
        /// </summary>
        /// <param name="range"></param>
        public NumericRangeComparer(double range)
        {
            Range = Math.Abs(range);
        }

        /// <inheritdoc />
        public override int Compare(double x, double y)
        {
            return Equals(x, y) ? 0 : x < y ? -1 : 1;
        }

        /// <inheritdoc />
        public override bool Equals(double x, double y)
        {
            return x.IsAlmostEqualByRange(y, Range);
        }
    }
}