using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class SimulationModelContext : ISimulationModelContext
    {
        /// <inheritdoc />
        public IList<IKineticSimulationModel> KineticSimulationModels { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisSimulationModel> MetropolisSimulationModels { get; set; }
    }
}
