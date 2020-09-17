using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.Constraints;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Defines the range of a <see cref="CrystalParameter" /> and if the value is mutable
    /// </summary>
    public readonly struct CrystalParameterRange
    {
        /// <summary>
        ///     Get the minimal value
        /// </summary>
        public double MinValue { get; }

        /// <summary>
        ///     Get the maximal value
        /// </summary>
        public double MaxValue { get; }

        /// <summary>
        ///     Get a boolean flag if the value is immutable due to the crystal context
        /// </summary>
        public bool IsContextImmutable { get; }

        /// <summary>
        ///     Defines a boolean flag if the value is always immutable with min equal to max
        /// </summary>
        public bool IsAlwaysImmutable => MinValue.AlmostEqualByRange(MaxValue);

        /// <summary>
        ///     Creates a new <see cref="CrystalParameterRange" /> with a minimal and maximal value
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="isContextImmutable"></param>
        public CrystalParameterRange(double minValue, double maxValue, bool isContextImmutable)
        {
            IsContextImmutable = isContextImmutable;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        /// <summary>
        ///     Creates a <see cref="CrystalParameter" /> that defines the smallest possible setting
        /// </summary>
        /// <returns></returns>
        public CrystalParameter GetMinimalParameter() => new CrystalParameter(MinValue, IsContextImmutable);

        /// <summary>
        ///     Creates a <see cref="NumericConstraint" /> instance from the contained values and a <see cref="NumericComparer" />
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="includeMin"></param>
        /// <param name="includeMax"></param>
        /// <returns></returns>
        public NumericConstraint ToNumericConstraint(NumericComparer comparer, bool includeMin = true, bool includeMax = true) =>
            new NumericConstraint(includeMin, MinValue, MaxValue, includeMax, comparer);
    }
}