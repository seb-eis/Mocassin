using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Basic;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Simulations
{
    /// <inheritdoc cref="IMetropolisSimulation" />
    public class MetropolisSimulation : SimulationBase, IMetropolisSimulation
    {
        /// <inheritdoc />
        IReadOnlyList<IMetropolisTransition> IMetropolisSimulation.Transitions => Transitions;

        /// <inheritdoc />
        public double RelativeBreakTolerance { get; set; }

        /// <inheritdoc />
        public int BreakSampleLength { get; set; }

        /// <inheritdoc />
        public int BreakSampleIntervalMcs { get; set; }

        /// <inheritdoc />
        public int ResultSampleMcs { get; set; }

        /// <summary>
        ///     The metropolis transitions attached to this simulation
        /// </summary>
        [UseTrackedData]
        public List<IMetropolisTransition> Transitions { get; set; }

        /// <inheritdoc />
        public override string ObjectName => "Metropolis Simulation";

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IMetropolisSimulation>(obj) is { } simulation))
                return null;

            base.PopulateFrom(obj);
            Transitions = (simulation.Transitions ?? new List<IMetropolisTransition>()).ToList();
            RelativeBreakTolerance = simulation.RelativeBreakTolerance;
            BreakSampleLength = simulation.BreakSampleLength;
            BreakSampleIntervalMcs = simulation.BreakSampleIntervalMcs;
            ResultSampleMcs = simulation.ResultSampleMcs;
            return this;
        }
    }
}