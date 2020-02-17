using System.Collections.Generic;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a kinetic simulation model that holds the relationship information between transitions and simulation
    ///     definitions
    /// </summary>
    public interface IKineticSimulationModel : ISimulationModel
    {
        /// <summary>
        ///     The kinetic simulation this model is based upon
        /// </summary>
        IKineticSimulation Simulation { get; set; }

        /// <summary>
        ///     The list of transition models that are valid in this simulation model
        /// </summary>
        IList<IKineticTransitionModel> TransitionModels { get; set; }

        /// <summary>
        ///     Kinetic mapping assign matrix that assigns each position index/particle index combination its valid kinetic mapping
        ///     models
        /// </summary>
        /// <remarks> Order on C side is [PositionId,ParticleId,ObjId] </remarks>
        IKineticMappingModel[,,] MappingAssignMatrix { get; set; }

        /// <summary>
        ///     The list of all existing jump models in the kinetic simulation model that finalized the actual transition behaviour
        ///     in the simulation
        /// </summary>
        IList<IKineticLocalJumpModel> LocalJumpModels { get; set; }
    }
}