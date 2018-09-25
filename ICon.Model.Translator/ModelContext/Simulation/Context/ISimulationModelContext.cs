using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a simulation model context that holds the relationship between simulations and affiliated model information
    /// </summary>
    public interface ISimulationModelContext
    {
        /// <summary>
        /// The list of kinetic simulation models that exist in the project context
        /// </summary>
        IList<IKineticSimulationModel> KineticSimulationModels { get; set; }

        /// <summary>
        /// The list of metropolis simulation models that exist in the project context
        /// </summary>
        IList<IMetropolisSimulationModel> MetropolisSimulationModels { get; set; }
    }
}
