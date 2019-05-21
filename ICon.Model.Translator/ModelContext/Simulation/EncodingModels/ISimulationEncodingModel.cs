using System.Collections.Generic;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Mobility type enum that encodes different kinds of possible mobility behaviors
    /// </summary>
    public enum MobilityType
    {
        Immobile,
        Mobile,
        Selectable
    }

    /// <summary>
    ///     Simulation encoding model for the simulation that contains mappings and encoding objects required by the simulator
    ///     application
    /// </summary>
    public interface ISimulationEncodingModel
    {
        /// <summary>
        ///     Dictionary that maps the transition models of the simulation model onto the simulation jump collection index
        /// </summary>
        IDictionary<ITransitionModel, int> TransitionModelToJumpCollectionId { get; set; }

        /// <summary>
        ///     Dictionary that maps the transition mapping models of the simulation model onto the simulation jump direction index
        /// </summary>
        IDictionary<ITransitionMappingModel, int> TransitionMappingToJumpDirectionId { get; set; }

        /// <summary>
        ///     Dictionary that maps the transition rule models onto their affiliated rule-related electric field factor values
        /// </summary>
        IDictionary<ITransitionRuleModel, double> TransitionRuleToElectricFieldFactors { get; set; }

        /// <summary>
        ///     List that maps the position index to a mobility type set that contains the kind of mobility for each particle
        ///     index
        /// </summary>
        IList<MobilityType[]> PositionIndexToMobilityTypesSet { get; set; }

        /// <summary>
        ///     Dictionary that maps the transition mapping models onto their affiliated direction-related electric field factor
        ///     values in [eV m / V]
        /// </summary>
        IDictionary<ITransitionMappingModel, double> TransitionMappingToElectricFieldFactors { get; set; }

        /// <summary>
        ///     The jump count table that assigns each position id + particle id combination the number of selectable jumps
        /// </summary>
        int[,] JumpCountMappingTable { get; set; }

        /// <summary>
        ///     The jump index assign table that assigns each position id + particle id + local direction id its valid jump
        ///     direction id
        /// </summary>
        int[,,] JumpDirectionIdMappingTable { get; set; }
    }
}