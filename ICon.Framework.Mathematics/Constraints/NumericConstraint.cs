using ICon.Framework.Constraints;
using ICon.Mathematics.Comparers;

namespace ICon.Mathematics.Constraints
{
    /// <summary>
    ///     Numeric constraint class that restricts the definition range of a double value through a numeric comparer
    /// </summary>
    public class NumericConstraint : ValueConstraint<double>
    {
        /// <summary>
        ///     The double comparator object
        /// </summary>
        public NumericComparer Comparer { get; set; }

        /// <inheritdoc />
        public NumericConstraint(bool minIsIncluded, double minValue, double maxValue, bool maxIsIncluded, NumericComparer comparer)
            : base(minIsIncluded, minValue, maxValue, maxIsIncluded)
        {
            Comparer = comparer;
        }

        /// <inheritdoc />
        public override bool IsValid(double sourceValue)
        {
            var minCompare = Comparer.Compare(sourceValue, MinValue);
            if (minCompare == -1 || minCompare == 0 && !MinIsIncluded)
                return false;

            var maxCompare = Comparer.Compare(sourceValue, MaxValue);
            return maxCompare != 1 && (maxCompare != 0 || MaxIsIncluded);
        }
    }
}