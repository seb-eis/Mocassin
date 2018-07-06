using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.Transitions;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Represents a specialized custom simulation that describes the reference data for a kinetic monte carlo routine
    /// </summary>
    public interface IKineticSimulation : IModelObject
    {
        /// <summary>
        /// The electric field direction as a read only fractional vector
        /// </summary>
        Fractional3D ElectricFieldVector { get; }

        /// <summary>
        /// The normalization probability that is used for in-simulation normalization of all jump attempts
        /// </summary>
        double NormalizationProbability { get; }

        /// <summary>
        /// The kinetic simulation flags that define on/off settings of this simulation
        /// </summary>
        KineticSimulationFlags KineticFlags { get; }

        /// <summary>
        /// Get an enumerable seqeunce with all transitions attachted to this simulation
        /// </summary>
        /// <returns></returns>
        IEnumerable<IKineticTransition> GetTransitions();
    }
}
