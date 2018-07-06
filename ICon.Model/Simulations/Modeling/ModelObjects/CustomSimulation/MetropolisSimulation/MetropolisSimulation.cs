using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using ICon.Model.Basic;
using ICon.Model.Transitions;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Enum for the metropolis specific simulation flags
    /// </summary>
    public enum MetropolisSimulationFlags
    {
        
    }

    /// <summary>
    /// Basic simulation model object that carries all reference data required to describe a single custom metropolis simulation
    /// </summary>
    [DataContract]
    public class MetropolisSimulation : CustomSimulation, IMetropolisSimulation
    {
        /// <summary>
        /// The relative break tolerance of the MMC simulation
        /// </summary>
        [DataMember]
        public double RelativeBreakTolerance { get; set; }

        /// <summary>
        /// The sample pool length used for averaging of break values
        /// </summary>
        [DataMember]
        public int BreakSampleLength { get; set; }

        /// <summary>
        /// The number of mcs in between pushing of values to the break sample pool
        /// </summary>
        [DataMember]
        public int BreakSampleIntervalMcs { get; set; }

        /// <summary>
        /// The number of MCS used after reaching break conditions to generate the results
        /// </summary>
        [DataMember]
        public int ResultSampleMcs { get; set; }

        /// <summary>
        /// The metropolis specific simulation flags
        /// </summary>
        [DataMember]
        public MetropolisSimulationFlags MetropolisFlags { get; set; }

        /// <summary>
        /// The metropolis transitions attached to this simulation
        /// </summary>
        [DataMember]
        [LinkableByIndex]
        public List<IMetropolisTransition> Transitions { get; set; }

        /// <summary>
        /// Get a string representing the model object name
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Metropolis Simulation'";
        }

        /// <summary>
        /// Get all metropolis transitions attached to this simulation
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMetropolisTransition> GetTransitions()
        {
            return Transitions.AsEnumerable();
        }
    }
}
