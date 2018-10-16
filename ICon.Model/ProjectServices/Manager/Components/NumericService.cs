using ICon.Mathematics.Comparers;

namespace ICon.Model.ProjectServices
{
    /// <inheritdoc />
    public class NumericService : INumericService
    {
        /// <summary>
        ///     The numeric settings object used to create the numeric comparer objects
        /// </summary>
        protected BasicNumericSettings Settings { get; set; }

        /// <inheritdoc />
        public NumericComparer UlpComparer { get; protected set; }

        /// <inheritdoc />
        public NumericComparer RangeComparer { get; protected set; }

        /// <inheritdoc />
        public NumericComparer RelativeComparer { get; protected set; }

        /// <inheritdoc />
        public int CompUlp => Settings.UlpValue;

        /// <inheritdoc />
        public double CompRange => Settings.RangeValue;

        /// <inheritdoc />
        public double CompFactor => Settings.FactorValue;

        /// <summary>
        ///     Creates new service object from settings data
        /// </summary>
        /// <param name="settings"></param>
        public NumericService(BasicNumericSettings settings)
        {
            Settings = settings;
            UlpComparer = NumericComparer.CreateUlp(settings.UlpValue);
            RangeComparer = NumericComparer.CreateRanged(settings.RangeValue);
            RelativeComparer = NumericComparer.CreateRelative(settings.FactorValue);
        }
    }
}