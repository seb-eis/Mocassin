using Mocassin.Mathematics.Comparers;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Represents a service for numeric floating point comparisons with specific tolerances for equality
    /// </summary>
    public interface INumericService
    {
        /// <summary>
        ///     The number of ULP steps in the ulp comparer
        /// </summary>
        int ComparisonUlp { get; }

        /// <summary>
        ///     The absolute comparison range of the range comparer
        /// </summary>
        double ComparisonRange { get; }

        /// <summary>
        ///     The relative comparison factor of the relative comparer
        /// </summary>
        double ComparisonFactor { get; }

        /// <summary>
        ///     The ULP based double comparer, fastest but fails during almost zero comparisons
        /// </summary>
        NumericComparer UlpComparer { get; }

        /// <summary>
        ///     The range based double comparer, relatively fast and able to compare to zero, but limited to a very specific value
        ///     range
        /// </summary>
        NumericComparer RangeComparer { get; }

        /// <summary>
        ///     The factor based relative double comparer, slow and not able to compare to zero, but usually the most flexible one
        /// </summary>
        NumericComparer RelativeComparer { get; }
    }
}