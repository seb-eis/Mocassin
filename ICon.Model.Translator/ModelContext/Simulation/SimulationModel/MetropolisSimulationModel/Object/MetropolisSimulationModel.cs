﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IMetropolisSimulationModel" />
    public class MetropolisSimulationModel : SimulationModel, IMetropolisSimulationModel
    {
        /// <inheritdoc />
        public IMetropolisSimulation Simulation { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisTransitionModel> TransitionModels { get; set; }

        /// <inheritdoc />
        public IMetropolisMappingModel[,,] MappingAssignMatrix { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisLocalJumpModel> LocalJumpModels { get; set; }

        /// <summary>
        ///     Create new metropolis simulation model with empty tracking model
        /// </summary>
        public MetropolisSimulationModel()
        {
            SimulationTrackingModel = ModelContext.SimulationTrackingModel.GetEmpty();
        }

        /// <inheritdoc />
        public override double MaxAttemptFrequency { get; set; }

        /// <inheritdoc />
        public override IEnumerable<ITransitionModel> GetTransitionModels()
        {
            return TransitionModels.AsEnumerable();
        }

        /// <inheritdoc />
        public override IEnumerable<ILocalJumpModel> GetLocalJumpModels()
        {
            return LocalJumpModels.AsEnumerable();
        }
    }
}