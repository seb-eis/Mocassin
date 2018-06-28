using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Simulation data manager implementation that provided read only access to the simulation model data object
    /// </summary>
    public class SimulationDataManager : ModelDataManager<SimulationModelData>, ISimulationDataPort
    {
        /// <summary>
        /// Creates a new simulation data manager adapter for the provided model data object
        /// </summary>
        /// <param name="data"></param>
        public SimulationDataManager(SimulationModelData data) : base(data)
        {
        }
    }
}
