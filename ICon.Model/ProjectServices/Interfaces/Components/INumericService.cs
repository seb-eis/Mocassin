using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.Comparers;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Represents a service for numeric floating point comparisons with specific tolerances for equality
    /// </summary>
    public interface INumericService
    {
        /// <summary>
        /// The number of ULP steps in the ulp comparer
        /// </summary>
        int CompUlp { get; }

        /// <summary>
        /// The absolut comparisons range of the range comparer
        /// </summary>
        double CompRange { get; }

        /// <summary>
        /// The relative comparisons factor of the relative comparer
        /// </summary>
        double CompFactor { get; }

        /// <summary>
        /// The ULP based double comparer, fastest but fails durng almost zero comparisons
        /// </summary>
        NumericComparer UlpComparer { get; }

        /// <summary>
        /// The range based double comparer, relatively fast and able to compare to zero, but limited to a very specific value range
        /// </summary>
        NumericComparer RangeComparer { get; }

        /// <summary>
        /// The factor based relative double comparer, slow and not able to compare to zero, but usually the most flexible one
        /// </summary>
        NumericComparer RelativeComparer { get; }
    }
}
