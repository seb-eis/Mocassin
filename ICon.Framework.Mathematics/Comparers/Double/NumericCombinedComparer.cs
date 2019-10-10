using System;

namespace Mocassin.Mathematics.Comparers
{
    /// <summary>
    ///     A <see cref="NumericComparer"/> that is build from one zero safe comparer and a main comparer
    /// </summary>
    public class NumericCombinedComparer : NumericComparer
    {
        /// <summary>
        ///     Get the <see cref="NumericComparer"/> for zero values
        /// </summary>
        public NumericComparer ZeroComparer { get; }

        /// <summary>
        ///     Get the main <see cref="NumericComparer"/>
        /// </summary>
        public NumericComparer MainComparer { get; }

        /// <inheritdoc />
        public override bool IsZeroCompatible => true;

        /// <summary>
        ///     Combines a zero safe and any other main <see cref="NumericComparer"/> to a combined multi purpose one that can handle all cases
        /// </summary>
        /// <param name="zeroComparer"></param>
        /// <param name="mainComparer"></param>
        public NumericCombinedComparer(NumericComparer zeroComparer, NumericComparer mainComparer)
        {
            ZeroComparer = zeroComparer ?? throw new ArgumentNullException(nameof(zeroComparer));
            MainComparer = mainComparer ?? throw new ArgumentNullException(nameof(mainComparer));
            if (!ZeroComparer.IsZeroCompatible) throw new ArgumentException("Zero comparer is not marked as zero safe.");
        }

        /// <inheritdoc />
        public override int Compare(double lhs, double rhs)
        {
            if (lhs == rhs) return 0;
            if (ZeroComparer.Equals(lhs, 0) || ZeroComparer.Equals(rhs, 0)) return ZeroComparer.Compare(lhs, rhs);
            return MainComparer.Compare(lhs, rhs);
        }

        /// <inheritdoc />
        public override bool Equals(double x, double y)
        {
            return Compare(x, y) == 0;
        }
    }
}