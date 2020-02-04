using System.Collections.ObjectModel;
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
    [XmlRoot]
    public class MetropolisSimulationData : SimulationBaseData
    {
        private double relativeBreakTolerance;
        private int breakSampleLength = 1000;
        private int breakSampleIntervalMcs = 100;
        private int resultSampleMcs = 200;
        private ObservableCollection<ModelObjectReference<MetropolisTransition>> transitions = new ObservableCollection<ModelObjectReference<MetropolisTransition>>();

        /// <summary>
        ///     Get or set the relative break tolerance value
        /// </summary>
        [XmlAttribute]
        public double RelativeBreakTolerance
        {
            get => relativeBreakTolerance;
            set => SetProperty(ref relativeBreakTolerance, value);
        }

        /// <summary>
        ///     Get or set the sample length for the break
        /// </summary>
        [XmlAttribute]
        public int BreakSampleLength
        {
            get => breakSampleLength;
            set => SetProperty(ref breakSampleLength, value);
        }

        /// <summary>
        ///     Get or set the sample interval for the break
        /// </summary>
        [XmlAttribute]
        public int BreakSampleIntervalMcs
        {
            get => breakSampleIntervalMcs;
            set => SetProperty(ref breakSampleIntervalMcs, value);
        }

        /// <summary>
        ///     Get or set the result sample mcs
        /// </summary>
        [XmlAttribute]
        public int ResultSampleMcs
        {
            get => resultSampleMcs;
            set => SetProperty(ref resultSampleMcs, value);
        }

        /// <summary>
        ///     Get or set the list of metropolis transitions active in the simulation
        /// </summary>
        [XmlArray]
        public ObservableCollection<ModelObjectReference<MetropolisTransition>> Transitions
        {
            get => transitions;
            set => SetProperty(ref transitions, value);
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