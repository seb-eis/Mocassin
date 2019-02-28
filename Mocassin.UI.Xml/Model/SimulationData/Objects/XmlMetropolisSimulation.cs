using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Simulations;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.Xml.SimulationModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Simulations.IMetropolisSimulation" /> model object creation
    /// </summary>
    [XmlRoot("MetropolisSimulation")]
    public class XmlMetropolisSimulation : XmlSimulationBase
    {
        /// <summary>
        ///     Get or set the relative break tolerance value
        /// </summary>
        [XmlAttribute("BreakTolerance")]
        public double RelativeBreakTolerance { get; set; }

        /// <summary>
        ///     Get or set the sample length for the break
        /// </summary>
        [XmlAttribute("BreakSampleLength")]
        public int BreakSampleLength { get; set; }

        /// <summary>
        ///     Get or set the sample interval for the break
        /// </summary>
        [XmlAttribute("BreakSampleInterval")]
        public int BreakSampleIntervalMcs { get; set; }

        /// <summary>
        ///     Get or set the result sample mcs
        /// </summary>
        [XmlAttribute("ResultSampleMcs")]
        public int ResultSampleMcs { get; set; }

        /// <summary>
        ///     Get or set the list of metropolis transitions active in the simulation
        /// </summary>
        [XmlArray("Transitions")]
        [XmlArrayItem("Transition")]
        public List<XmlMetropolisTransition> Transitions { get; set; }

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