using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.Comparers;

namespace ICon.Model.ProjectServices
{
    /// <summary>
    /// Numeric service of the settings manager to provide uniform numeric comparators during the model process
    /// </summary>
    public class NumericService : INumericService
    {
        /// <summary>
        /// The numeric settings object used to create the comparers
        /// </summary>
        protected BasicNumericSettings Settings { get; set; }

        /// <summary>
        /// Get the ulp comparer
        /// </summary>
        public NumericComparer UlpComparer { get; protected set; }

        /// <summary>
        /// Get the range comparer
        /// </summary>
        public NumericComparer RangeComparer { get; protected set; }

        /// <summary>
        /// Get the relative factor comparer
        /// </summary>
        public NumericComparer RelativeComparer { get; protected set; }

        /// <summary>
        /// Comparison steps of the ULP comparer
        /// </summary>
        public int CompUlp => Settings.UlpValue;

        /// <summary>
        /// Comparison range of the range comparer
        /// </summary>
        public double CompRange => Settings.RangeValue;

        /// <summary>
        /// Comparisons factor of the relative comparer
        /// </summary>
        public double CompFactor => Settings.FactorValue;

        /// <summary>
        /// Creates new service object from settings data
        /// </summary>
        /// <param name="data"></param>
        public NumericService(BasicNumericSettings settings)
        {
            Settings = settings;
            UlpComparer = NumericComparer.CreateUlp(settings.UlpValue);
            RangeComparer = NumericComparer.CreateRanged(settings.RangeValue);
            RelativeComparer = NumericComparer.CreateRelative(settings.FactorValue);
        }

    }
}
