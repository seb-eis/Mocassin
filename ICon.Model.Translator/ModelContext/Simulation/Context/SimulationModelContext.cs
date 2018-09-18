using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Simulation model context that carries the extended data context for simulation creation/evaluation
    /// </summary>
    public class SimulationModelContext : ISimulationModelContext
    {
        /// <summary>
        /// The list of kinetic simulation models that exist in the project context
        /// </summary>
        public IList<IKineticSimulationModel> KineticSimulationModels { get; set; }

        /// <summary>
        /// The list of metropolis simulation models that exist in the project context
        /// </summary>
        public IList<IMetropolisSimulationModel> MetropolisSimulationModels { get; set; }
    }
}
