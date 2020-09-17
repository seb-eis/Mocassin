namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Enum to describe the unit cell position status (Stable, unstable,...)
    /// </summary>
    public enum PositionStability
    {
        /// <summary>
        ///     The position is stable
        /// </summary>
        Stable,

        /// <summary>
        ///     The position is unstable and is occupied only during the S1 state
        /// </summary>
        Unstable
    }
}