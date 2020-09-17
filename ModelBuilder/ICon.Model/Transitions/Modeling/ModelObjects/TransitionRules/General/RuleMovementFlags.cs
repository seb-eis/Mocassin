using System;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Further specifies the type of the rule in terms of the physical process
    /// </summary>
    [Flags]
    public enum RuleMovementFlags
    {
        /// <summary>
        ///     Movement contains unsupported or unrecognized steps
        /// </summary>
        HasUnsupportedMovement = 1,

        /// <summary>
        ///     Movement contains physical movement
        /// </summary>
        HasPhysicalMovement = 1 << 1,

        /// <summary>
        ///     Movement contains property exchange movement
        /// </summary>
        HasPropertyMovement = 1 << 2,

        /// <summary>
        ///     Movement contains vacancy movement
        /// </summary>
        HasVacancyMovement = 1 << 3,

        /// <summary>
        ///     Movement contains vehicle movement
        /// </summary>
        HasVehicleMovement = 1 << 4,

        /// <summary>
        ///     Movement contains physical atom pushing movement
        /// </summary>
        HasChainedMovement = 1 << 5,

        /// <summary>
        ///     Movement is recognized as an exchange
        /// </summary>
        IsExchange = 1 << 6,

        /// <summary>
        ///     Movement is recognized as a migration
        /// </summary>
        IsMigration = 1 << 7,

        /// <summary>
        ///     Movement is recognized as an association/dissociation type
        /// </summary>
        IsAssociation = 1 << 8
    }
}