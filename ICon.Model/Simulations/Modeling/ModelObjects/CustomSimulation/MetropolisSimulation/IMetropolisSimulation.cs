using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;
using ICon.Model.Transitions;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Represents a custom simulation that contains all reference data required to describe a metropolis simulation
    /// </summary>
    public interface IMetropolisSimulation : IModelObject
    {
        /// <summary>
        /// The energy value tolerance used for pre-target-mcsp simulation breaks
        /// </summary>
        double RelativeBreakTolerance { get; }

        /// <summary>
        /// Defines the length of the data pool used to check break conditions
        /// </summary>
        int BreakSampleLength { get; }

        /// <summary>
        /// Defines the interval of value sampling for breaking in MCS, i.e. how often the simulation pushes a value to the break pool
        /// </summary>
        int BreakSampleIntervalMcs { get; }

        /// <summary>
        /// Defines how many MCS the simulation will use after reaching break conditions to create averaged final results
        /// </summary>
        int ResultSampleMcs{ get; }

        /// <summary>
        /// Get the metropolis specific simulation flags
        /// </summary>
        MetropolisSimulationFlags MetropolisFlags { get; }

        /// <summary>
        /// Get all metropolis transitions attached to this simulation
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMetropolisTransition> GetTransitions();
    }
}
