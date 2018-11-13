using System;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Fully describes the behaviour of a transition mapping model in the context of a kinetic simulation model
    /// </summary>
    public interface IKineticLocalJumpModel : IModelComponent, IEquatable<IKineticLocalJumpModel>
    {
        /// <summary>
        ///     The kinetic mapping model that describes the geometry of the local jump model
        /// </summary>
        IKineticMappingModel MappingModel { get; set; }

        /// <summary>
        ///     The kinetic rule model that is valid for the jump model
        /// </summary>
        IKineticRuleModel RuleModel { get; set; }

        /// <summary>
        /// Defines the fraction of the electric normalized influence the jump model sees during simulation
        /// </summary>
        /// <remarks> Normalized influence is always defined as an influence of 1eV (1C*1Ang*1V/Ang) in the simulation field direction </remarks>
        double NormalizedElectricFieldInfluence { get; set; }

        /// <summary>
        /// The electric field influence factor that is bound to the transition rule on simulation database creation
        /// </summary>
        /// <remarks> Describes the direction of the charge movement as a factor of [-1] or [1] </remarks>
        double ElectricFieldRuleFactor { get; }

        /// <summary>
        /// The electric field influence factor that is bound to the kinetic transition mapping on simulation database creation
        /// </summary>
        /// <remarks> Has to be the absolute value of the electric normalized influence fraction </remarks>
        double ElectricFieldMappingFactor { get; }

        /// <summary>
        /// The cartesian charge transport vector of the local jump model that describes [charge*movement] in units of [C*Ang]
        /// </summary>
        Cartesian3D ChargeTransportVector { get; set; }
    }
}