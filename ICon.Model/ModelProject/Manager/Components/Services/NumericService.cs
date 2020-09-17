using Mocassin.Mathematics.Comparer;

namespace Mocassin.Model.ModelProject
{
    /// <inheritdoc />
    public class NumericService : INumericService
    {
        /// <summary>
        ///     The numeric settings object used to create the numeric comparer objects
        /// </summary>
        protected MocassinNumericSettings Settings { get; set; }

        /// <inheritdoc />
        public NumericComparer UlpComparer { get; protected set; }

        /// <inheritdoc />
        public NumericComparer RangeComparer { get; protected set; }

        /// <inheritdoc />
        public NumericComparer RelativeComparer { get; protected set; }

        /// <inheritdoc />
        public NumericComparer ComboComparer { get; protected set; }

        /// <inheritdoc />
        public int ComparisonUlp => Settings.UlpValue;

        /// <inheritdoc />
        public double ComparisonRange => Settings.RangeValue;

        /// <inheritdoc />
        public double ComparisonFactor => Settings.FactorValue;

        /// <summary>
        ///     Creates new service object from settings data
        /// </summary>
        /// <param name="settings"></param>
        public NumericService(MocassinNumericSettings settings)
        {
            Settings = settings;
            UlpComparer = NumericComparer.CreateUlp(settings.UlpValue);
            RangeComparer = NumericComparer.CreateRanged(settings.RangeValue);
            RelativeComparer = NumericComparer.CreateRelative(settings.FactorValue);
            ComboComparer = new NumericCombinedComparer(RangeComparer, RelativeComparer);
        }
    }
}