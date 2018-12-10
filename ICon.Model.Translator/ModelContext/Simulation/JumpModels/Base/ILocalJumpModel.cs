using System.Collections.Generic;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Fully describes the behaviour of a transition mapping model in the context of a simulation model
    /// </summary>
    public interface ILocalJumpModel : IModelComponent
    {
        /// <summary>
        ///     Get the basic unspecified rule model that the jump model is based upon
        /// </summary>
        ITransitionRuleModel RuleModelBase { get; }

        /// <summary>
        ///     Get the basic unspecified mapping model that the jump model is based upon
        /// </summary>
        ITransitionMappingModel MappingModelBase { get; }

        /// <summary>
        ///     The electric field influence factor that is bound to the transition rule on simulation database creation
        /// </summary>
        /// <remarks> Describes the direction of the charge movement as a factor of [-1;0;1] </remarks>
        double ElectricFieldRuleInfluence { get; }

        /// <summary>
        ///     The electric field influence factor that is bound to the transition mapping on simulation database creation
        /// </summary>
        /// <remarks> Describes the fraction of the normalized field that is applied if the transport charge moves in positive direction </remarks>
        double ElectricFieldMappingFactor { get; set; }

        /// <summary>
        ///     Get the mobility type that the local jump will produce for the passed position id + particle id combination
        /// </summary>
        /// <param name="positionId"></param>
        /// <param name="particleId"></param>
        /// <returns></returns>
        MobilityType GetMobilityType(int positionId, int particleId);
    }
}