using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.SimulationModel
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the
    ///     <see cref="Mocassin.Model.Simulations.ISimulationManager" />
    ///     system
    /// </summary>
    [XmlRoot("SimulationModel")]
    public class SimulationModelGraph : ModelManagerGraph
    {
        private ObservableCollection<MetropolisSimulationGraph> metropolisSimulations;
        private ObservableCollection<KineticSimulationGraph> kineticSimulations;

        /// <summary>
        ///     Get or set the list of metropolis simulations
        /// </summary>
        [XmlArray("MetropolisSimulations")]
        [XmlArrayItem("MetropolisSimulation")]
        public ObservableCollection<MetropolisSimulationGraph> MetropolisSimulations
        {
            get => metropolisSimulations;
            set => SetProperty(ref metropolisSimulations, value);
        }

        /// <summary>
        ///     Get or set the list of metropolis simulations
        /// </summary>
        [XmlArray("KineticSimulations")]
        [XmlArrayItem("KineticSimulation")]
        public ObservableCollection<KineticSimulationGraph> KineticSimulations
        {
            get => kineticSimulations;
            set => SetProperty(ref kineticSimulations, value);
        }

        /// <summary>
        ///     Creates new <see cref="SimulationModelGraph" /> with empty component lists
        /// </summary>
        public SimulationModelGraph()
        {
            MetropolisSimulations = new ObservableCollection<MetropolisSimulationGraph>();
            KineticSimulations = new ObservableCollection<KineticSimulationGraph>();
        }


        /// <inheritdoc />
        public override IEnumerable<IModelParameter> GetInputParameters()
        {
            yield break;
        }

        /// <inheritdoc />
        public override IEnumerable<IModelObject> GetInputObjects()
        {
            return MetropolisSimulations.Select(x => x.GetInputObject())
                .Concat(KineticSimulations.Select(x => x.GetInputObject()));
        }
    }
}