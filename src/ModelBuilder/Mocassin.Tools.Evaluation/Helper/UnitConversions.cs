namespace Mocassin.Tools.Evaluation.Helper
{
    /// <summary>
    ///     Stores the unit conversion information required to convert the internally used unit to typical SI representation
    /// </summary>
    public static class UnitConversions
    {
        /// <summary>
        ///     Length helper class
        /// </summary>
        public static class Length
        {
            /// <summary>
            ///     Conversion factor from [Ang] to [m]
            /// </summary>
            public static double AngstromToMeter { get; } = 1e-10;

            /// <summary>
            ///     Conversion factor from [m] to [Ang]
            /// </summary>
            public static double MeterToAngstrom { get; } = 1 / AngstromToMeter;
        }
    }
}