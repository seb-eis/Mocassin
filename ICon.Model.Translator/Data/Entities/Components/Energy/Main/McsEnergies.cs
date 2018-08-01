using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// The mccs energy component that carries all encoded simulation data affiliated with energy modelling
    /// </summary>
    public class McsEnergies : McsComponent
    {
        /// <summary>
        /// The list of cluster energy matrices for the simulation
        /// </summary>
        public List<ClusterEnergyTable> ClusterEnergyMatrices { get; set; }

        /// <summary>
        /// The list of pair energy matrices for the simulation
        /// </summary>
        public List<PairEnergyTable> PairEnergyMatrices { get; set; }
    }
}
