using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class SimulationModelContext : ISimulationModelContext
    {
        /// <inheritdoc />
        public IList<IKineticSimulationModel> KineticSimulationModels { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisSimulationModel> MetropolisSimulationModels { get; set; }

        /// <inheritdoc />
        public ISimulationModel FindSimulationModel(ISimulation simulation)
        {
            switch (simulation)
            {
                case IKineticSimulation kineticSimulation:
                    return KineticSimulationModels?.SingleOrDefault(x => x.Simulation == kineticSimulation);

                case IMetropolisSimulation metropolisSimulation:
                    return MetropolisSimulationModels?.SingleOrDefault(x => x.Simulation == metropolisSimulation);
            }
            throw new ArgumentException("Passed simulation type is not supported");
        }
    }
}