using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Fully describes the behaviour of a transition mapping model in the context of a kinetic simulation model
    /// </summary>
    public interface IKineticLocalJumpModel : IModelComponent
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
        ///     The electric field projection vector of the local jump model
        /// </summary>
        Fractional3D ElectricFieldProjectionVector { get; set; }

        /// <summary>
        ///     The fraction [0.0,1.0] of the absolute field magnitude applied by this jump model
        /// </summary>
        double ElectricFieldInfluenceFraction { get; set; }

        /// <summary>
        ///     The effective transport charge described by the local jump model
        /// </summary>
        double TransportCharge { get; set; }
    }
}