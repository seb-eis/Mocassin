using System.Dynamic;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Stores the unit conversion information required to convert the internally used unit to typical SI representation
    /// </summary>
    public static class UnitConversions
    {
        public static class Length
        {
            /// <summary>
            ///     Conversion factor from [Ang] to [m]
            /// </summary>
            public static double AngToMeter { get; } = 1e-10;

            /// <summary>
            ///     Conversion factor from [m] to [Ang]
            /// </summary>
            public static double MeterToAng { get; } = 1 / AngToMeter;
        }
    }
}