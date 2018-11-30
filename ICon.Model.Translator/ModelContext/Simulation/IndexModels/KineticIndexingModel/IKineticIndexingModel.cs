using System.Collections.Generic;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Kinetic indexing model for the simulation that maps the kinetic simulation model context instances onto their
    ///     affiliated simulation index
    /// </summary>
    public interface IKineticIndexingModel
    {
        /// <summary>
        ///     The kinetic simulation model that the index model is valid for
        /// </summary>
        IKineticSimulationModel SimulationModel { get; set; }

        /// <summary>
        ///     Dictionary that maps the transition models of the simulation model onto the simulation jump collection index
        /// </summary>
        IDictionary<IKineticTransitionModel, int> TransitionModelToJumpCollectionId { get; set; }

        /// <summary>
        ///     Dictionary that mas the kinetic mapping models of the simulation model onto the simulation jump direction index
        /// </summary>
        IDictionary<IKineticMappingModel, int> TransitionMappingToJumpDirectionId { get; set; }

        /// <summary>
        /// The jump count table that assigns each position id + particle id combination the number of selectable jumps
        /// </summary>
        int[,] JumpCountTable { get; set; }

        /// <summary>
        /// The jump index assign table that assigns each position id + particle id + local direction id its valid jump direction id
        /// </summary>
        int[,,,] JumpIndexAssignTable { get; set; }
    }
}