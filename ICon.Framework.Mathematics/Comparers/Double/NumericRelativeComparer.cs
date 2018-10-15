using System;
using ICon.Mathematics.Extensions;

namespace ICon.Mathematics.Comparers
{
    /// <summary>
    ///     Double relative comparer class that performs almost equal comparisons with a relative factor (Not zero safe)
    /// </summary>
    public class NumericRelativeComparer : NumericComparer
    {
        /// <summary>
        ///     Shared zero safe flag
        /// </summary>
        private static readonly bool ZeroSaveValue = false;

        /// <summary>
        ///     The absolute tolerance range value
        /// </summary>
        public double RelativeRange { get; protected set; }

        /// <inheritdoc />
        public override bool IsZeroCompatible => ZeroSaveValue;

        /// <summary>
        ///     Creates a new relative range comparer with the specified tolerance factor
        /// </summary>
        /// <param name="range"></param>
        public NumericRelativeComparer(double range)
        {
            RelativeRange = Math.Abs(range);
        }

        /// <inheritdoc />
        public override int Compare(double x, double y)
        {
            return Equals(x, y) ? 0 : x < y ? -1 : 1;
        }

        /// <inheritdoc />
        public override bool Equals(double x, double y)
        {
            return x.IsAlmostEqualByRelative(y, RelativeRange);
        }
    }
}