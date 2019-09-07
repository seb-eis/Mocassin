using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Simulations;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.SimulationModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Simulations.IMetropolisSimulation" /> model object creation
    /// </summary>
    [XmlRoot("MetropolisSimulation")]
    public class MetropolisSimulationGraph : SimulationBaseGraph
    {
        private double relativeBreakTolerance;
        private int breakSampleLength = 1000;
        private int breakSampleIntervalMcs = 100;
        private int resultSampleMcs = 200;
        private List<ModelObjectReferenceGraph<MetropolisTransition>> transitions;

        /// <summary>
        ///     Get or set the relative break tolerance value
        /// </summary>
        [XmlAttribute("BreakTolerance")]
        public double RelativeBreakTolerance
        {
            get => relativeBreakTolerance;
            set => SetProperty(ref relativeBreakTolerance, value);
        }

        /// <summary>
        ///     Get or set the sample length for the break
        /// </summary>
        [XmlAttribute("BreakSampleLength")]
        public int BreakSampleLength
        {
            get => breakSampleLength;
            set => SetProperty(ref breakSampleLength, value);
        }

        /// <summary>
        ///     Get or set the sample interval for the break
        /// </summary>
        [XmlAttribute("BreakSampleInterval")]
        public int BreakSampleIntervalMcs
        {
            get => breakSampleIntervalMcs;
            set => SetProperty(ref breakSampleIntervalMcs, value);
        }

        /// <summary>
        ///     Get or set the result sample mcs
        /// </summary>
        [XmlAttribute("ResultSampleMcs")]
        public int ResultSampleMcs
        {
            get => resultSampleMcs;
            set => SetProperty(ref resultSampleMcs, value);
        }

        /// <summary>
        ///     Get or set the list of metropolis transitions active in the simulation
        /// </summary>
        [XmlArray("Transitions")]
        [XmlArrayItem("Transition")]
        public List<ModelObjectReferenceGraph<MetropolisTransition>> Transitions
        {
            get => transitions;
            set => SetProperty(ref transitions, value);
        }

        /// <summary>
        ///     Creates new <see cref="MetropolisSimulationGraph" /> with empty component lists
        /// </summary>
        public MetropolisSimulationGraph()
        {
            Transitions = new List<ModelObjectReferenceGraph<MetropolisTransition>>();
        }

        /// <inheritdoc />
        protected override SimulationBase GetPreparedSpecifiedSimulation()
        {
            var obj = new MetropolisSimulation
            {
                RelativeBreakTolerance = RelativeBreakTolerance,
                BreakSampleLength = BreakSampleLength,
                BreakSampleIntervalMcs = BreakSampleIntervalMcs,
                ResultSampleMcs = ResultSampleMcs,
                Transitions = Transitions.Select(x => x.GetInputObject()).Cast<IMetropolisTransition>().ToList()
            };
            return obj;
        }
    }
}