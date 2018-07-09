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
    public class MetropolisSimulation : SimulationBase, IMetropolisSimulation
    {
        /// <summary>
        /// Get a read only interface access to the list of metropolis transitions for the simulation
        /// </summary>
        IReadOnlyList<IMetropolisTransition> IMetropolisSimulation.Transitions => Transitions;

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
        /// Populates this object from a model object interface and returns this object. Retruns null if the population operation failed
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IMetropolisSimulation>(base.PopulateFrom(obj)) is IMetropolisSimulation simulation)
            {
                Transitions = (simulation.Transitions ?? new List<IMetropolisTransition>()).ToList();
                RelativeBreakTolerance = simulation.RelativeBreakTolerance;
                BreakSampleLength = simulation.BreakSampleLength;
                BreakSampleIntervalMcs = simulation.BreakSampleIntervalMcs;
                ResultSampleMcs = simulation.ResultSampleMcs;
                MetropolisFlags = simulation.MetropolisFlags;
                return this;
            }
            return null;
        }
    }
}
