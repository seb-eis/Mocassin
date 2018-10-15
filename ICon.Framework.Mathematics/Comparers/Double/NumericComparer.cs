using System.Collections.Generic;

namespace ICon.Mathematics.Comparers
{
    /// <summary>
    ///     Abstract base class for all tolerance based double comparer objects
    /// </summary>
    public abstract class NumericComparer : Comparer<double>, IEqualityComparer<double>
    {
        /// <summary>
        ///     Flag that defines if the comparer can be safely used to compare to zero
        /// </summary>
        public abstract bool IsZeroCompatible { get; }

        /// <summary>
        ///     Creates a new ULP comparator object with the specified step count
        /// </summary>
        /// <param name="steps"></param>
        /// <returns></returns>
        public static NumericComparer CreateUlp(int steps)
        {
            return new NumericUlpComparer(steps);
        }

        /// <summary>
        ///     Creates a new relative comparator object with the specified relative factor
        /// </summary>
        /// <param name="relativeRange"></param>
        /// <returns></returns>
        public static NumericComparer CreateRelative(double relativeRange)
        {
            return new NumericRelativeComparer(relativeRange);
        }

        /// <summary>
        ///     Creates a new range comparator object with the specified absolute range
        /// </summary>
        /// <param name="absoluteRange"></param>
        /// <returns></returns>
        public static NumericComparer CreateRanged(double absoluteRange)
        {
            return new NumericRangeComparer(absoluteRange);
        }

        /// <summary>
        ///     Creates a new default tolerance comparer (range based, allows 1.0e-13 offset, overwrites Comparer.Default)
        /// </summary>
        /// <returns></returns>
        public new static NumericComparer Default()
        {
            return CreateRanged(1.0e-13);
        }

        /// <summary>
        ///     Compares two double values with the internally specified information
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public abstract override int Compare(double first, double second);

        /// <inheritdoc />
        public abstract bool Equals(double x, double y);

        /// <inheritdoc />
        public int GetHashCode(double obj)
        {
            return obj.GetHashCode();
        }
    }
}