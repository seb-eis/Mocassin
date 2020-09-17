namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Static class that contains constant values used within the 'C' simulator
    /// </summary>
    public static class SimulationConstants
    {
        /// <summary>
        ///     The index value that is generally used for an invalid index on simulation side
        /// </summary>
        public const int InvalidId = -1;

        /// <summary>
        ///     The index of invalid particles in the simulation
        /// </summary>
        public const byte InvalidParticleId = byte.MaxValue;

        /// <summary>
        ///     The jump count value if a species is completely immobile
        /// </summary>
        public const int JumpCountIfNotMobile = -1;

        /// <summary>
        ///     The jump count value if a species cannot be selected directly but can be dragged by another transition
        /// </summary>
        public const int JumpCountIfPassivelyMobile = 0;

        /// <summary>
        ///     The jump direction influence factor for the positive rule direction
        /// </summary>
        public const int PositiveRuleDirectionFactor = 1;

        /// <summary>
        ///     The jump direction influence factor for the negative rule direction
        /// </summary>
        public const int NegativeRuleDirectionFactor = -1;

        /// <summary>
        ///     The jump direction influence factor for the undefinable rule direction (e.g. metropolis case)
        /// </summary>
        public const int UndefinableRuleDirectionFactor = 0;
    }
}