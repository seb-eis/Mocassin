using System.Collections.Generic;

namespace Mocassin.Model.Translator.ModelContext
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