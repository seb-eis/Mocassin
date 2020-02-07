namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Enum that identifies specific variations of crystal systems e.g. hexagonal axes for trigonal type
    /// </summary>
    public enum CrystalSystemVariation
    {
        /// <summary>
        ///     System has not special variation or the variation is an origin choice
        /// </summary>
        NoneOrOriginChoice = 0,
        
        /// <summary>
        ///     System has a unique axis in A direction
        /// </summary>
        UniqueAxisA = 1,

        /// <summary>
        ///     System has a unique axis in B direction
        /// </summary>
        UniqueAxisB = 2,

        /// <summary>
        ///     System has a unique axis in C direction
        /// </summary>
        UniqueAxisC = 3,

        /// <summary>
        ///     System uses hexagonal coordinate axes
        /// </summary>
        HexagonalAxes = 4,

        /// <summary>
        ///     System uses rhombohedral coordinate axes
        /// </summary>
        RhombohedralAxes = 5
    }
}