using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents a kinetic simulation model that holds the relationship information between transitions and simulation
    ///     definitions
    /// </summary>
    public interface IKineticSimulationModel : IModelComponent
    {
        /// <summary>
        ///     The kinetic simulation this model is based upon
        /// </summary>
        IKineticSimulation Simulation { get; set; }

        /// <summary>
        ///     The normalized electric field vector in cartesian coordinates. Describes a field strength of 1 [V/Ang]
        /// </summary>
        Cartesian3D NormalizedElectricFieldVector { get; set; }

        /// <summary>
        ///     The list of transition models that are valid in this simulation model
        /// </summary>
        IList<IKineticTransitionModel> TransitionModels { get; set; }

        /// <summary>
        ///     The movement tracking model that describes how particle transition combinations are tracked
        /// </summary>
        IKineticTrackingModel KineticTrackingModel { get; set; }

        /// <summary>
        ///     The kinetic indexing model that describes the required indexing sets for the simulation on the unmanaged side
        /// </summary>
        IKineticIndexingModel KineticIndexingModel { get; set; }

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