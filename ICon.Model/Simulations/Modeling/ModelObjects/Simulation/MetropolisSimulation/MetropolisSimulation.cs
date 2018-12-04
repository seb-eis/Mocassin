using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Enum for the metropolis specific simulation flags
    /// </summary>
    [Flags]
    public enum MetropolisSimulationFlags
    {
    }

    /// <inheritdoc cref="IMetropolisSimulation" />
    [DataContract]
    public class MetropolisSimulation : SimulationBase, IMetropolisSimulation
    {
        /// <inheritdoc />
        [IgnoreDataMember]
        IReadOnlyList<IMetropolisTransition> IMetropolisSimulation.Transitions => Transitions;

        /// <inheritdoc />
        [DataMember]
        public double RelativeBreakTolerance { get; set; }

        /// <inheritdoc />
        [DataMember]
        public int BreakSampleLength { get; set; }

        /// <inheritdoc />
        [DataMember]
        public int BreakSampleIntervalMcs { get; set; }

        /// <inheritdoc />
        [DataMember]
        public int ResultSampleMcs { get; set; }

        /// <inheritdoc />
        [DataMember]
        public MetropolisSimulationFlags MetropolisFlags { get; set; }

        /// <summary>
        ///     The metropolis transitions attached to this simulation
        /// </summary>
        [DataMember]
        [IndexResolved]
        public List<IMetropolisTransition> Transitions { get; set; }

        /// <inheritdoc />
        public override string GetObjectName()
        {
            return "Metropolis Simulation";
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IMetropolisSimulation>(obj) is IMetropolisSimulation simulation))
                return null;

            base.PopulateFrom(obj);
            Transitions = (simulation.Transitions ?? new List<IMetropolisTransition>()).ToList();
            RelativeBreakTolerance = simulation.RelativeBreakTolerance;
            BreakSampleLength = simulation.BreakSampleLength;
            BreakSampleIntervalMcs = simulation.BreakSampleIntervalMcs;
            ResultSampleMcs = simulation.ResultSampleMcs;
            MetropolisFlags = simulation.MetropolisFlags;
            return this;
        }
    }
}