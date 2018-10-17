using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a kinetic simulation model that holds the relationship information between transitions and simulation definitions
    /// </summary>
    public interface IKineticSimulationModel : IModelComponent
    {
        /// <summary>
        /// The kinetic simulation this model is based upon
        /// </summary>
        IKineticSimulation Simulation { get; set; }

        /// <summary>
        /// The list of transition models that are valid in this simulation model
        /// </summary>
        IList<IKineticTransitionModel> TransitionModels { get; set; }

        /// <summary>
        /// The movement tracking model that describes how particle transition combinations are tracked
        /// </summary>
        IKineticTrackingModel KineticTrackingModel { get; set; }

        /// <summary>
        /// Kinetic mapping assign matrix that assigns each particle index position index combination its valid kinetic mapping models
        /// </summary>
        IKineticMappingModel[,,] MappingAssignMatrix { get; set; }
    }
}
