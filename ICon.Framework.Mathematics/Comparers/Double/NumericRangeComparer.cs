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
        ///     The absolute tolerance range value
        /// </summary>
        public double Range { get; protected set; }

        /// <inheritdoc />
        public override bool IsZeroCompatible => true;

        /// <summary>
        ///     Creates a new range comparer with the specified range
        /// </summary>
        /// <param name="range"></param>
        public NumericRangeComparer(double range)
        {
            Range = Math.Abs(range);
        }

        /// <inheritdoc />
        public override int Compare(double lhs, double rhs)
        {
            return Equals(lhs, rhs) ? 0 : lhs < rhs ? -1 : 1;
        }

        /// <inheritdoc />
        public override bool Equals(double x, double y)
        {
            return x.IsAlmostEqualByRange(y, Range);
        }
    }
}