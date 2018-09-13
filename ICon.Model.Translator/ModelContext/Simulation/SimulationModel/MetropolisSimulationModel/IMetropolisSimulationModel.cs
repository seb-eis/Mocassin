﻿using ICon.Model.Simulations;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a metropolis simulation model that holds the relationship information between transitions and simulation definitions
    /// </summary>
    public interface IMetropolisSimulationModel : IModelComponent
    {
        /// <summary>
        /// The metropolis simulation this model is based upon
        /// </summary>
        IMetropolisSimulation Simulation { get; set; }

        /// <summary>
        /// The list of transition models that are valid in this simulation model
        /// </summary>
        IList<IMetropolisTransitionModel> TransitionModels { get; set; }

        /// <summary>
        /// Metropolis mapping assign matrix that assigns each particle index position index combination its valid mapping models
        /// </summary>
        IMetropolisMappingModel[,,] MappingAssignMatrix { get; set; }
    }
}
